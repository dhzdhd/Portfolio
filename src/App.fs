module App

open Browser.Dom
open Fetch
open Thoth.Json
open System.Text.RegularExpressions

type GithubJsonRecord = {
    owner: string
    repo: string
    link: string
    description: string
    language: string
    stars: int
    forks: int
}

let mutable flag = false

module private Utils =
    let switchTheme (event: Browser.Types.Event) =
        if not flag then
            document.documentElement.setAttribute("theme", "dark")
            flag <- true
        else
            document.documentElement.setAttribute("theme", "light")
            flag <- false

module private Github =
    let updateCards (ghRecord: GithubJsonRecord) (index: int) = 
        let card = document.querySelector $"#card-%i{index + 1}" :?> Browser.Types.HTMLDivElement

        let link = document.querySelector $"#link-%i{index + 1}" :?> Browser.Types.HTMLLinkElement
        link.href <- ghRecord.link

        let title = card.querySelector ".card-title"
        title.textContent <- ghRecord.repo

        let desc = card.querySelector ".card-desc"
        desc.textContent <- if ghRecord.description.Equals "" then "No description" else ghRecord.description
        
        let language = card.querySelector ".card-lang"
        language.textContent <- ghRecord.language
        
        let stars = card.querySelector ".card-stars"
        stars.textContent <- ghRecord.stars.ToString ()

        let forks = card.querySelector ".card-forks"
        forks.textContent <- ghRecord.forks.ToString ()
        
        ()

    let getRepoInfo =
        fetch "https://gh-pinned-repos-5l2i19um3.vercel.app/?username=dhzdhd" []
        |> Promise.bind (fun res -> res.text())
        |> Promise.map(fun txt -> Regex.Matches (txt, "{.*?}"))
        |> Promise.map (fun lst ->
            for i in 0..lst.Count do
                let decoded = Decode.Auto.fromString<GithubJsonRecord> ((lst.Item i).ToString (), caseStrategy = CamelCase)
                match decoded with
                | Ok resp -> updateCards resp i
                | Error decodingError -> printfn $"{decodingError}"
        )
        |> ignore

module private Facts =
    let updateTodayText text =
        let todayFactText = document.querySelector(".today-fact-text") :?> Browser.Types.HTMLHeadingElement
        todayFactText.innerText <- text
        ()

    let updateRandomText text =
        let randomFactText = document.querySelector(".random-fact-text") :?> Browser.Types.HTMLHeadingElement
        randomFactText.innerText <- text
        ()

    let getFact (url:string) (factType:string) =
        fetch url []
        |> Promise.bind (fun res -> res.text())
        |> Promise.map (fun txt -> 
            let decoded = Decode.Auto.fromString<{|Text: string|}> (txt, caseStrategy = CamelCase)
            match decoded, factType with
            | Ok resp, "today" -> updateTodayText resp.Text
            | Ok resp, "random" -> updateRandomText resp.Text
            | Error decodingError, "today" -> updateTodayText decodingError
            | Error decodingError, "random" -> updateRandomText decodingError
            | _, _ -> ()
        )
        |> ignore

Facts.getFact "https://uselessfacts.jsph.pl/today.json?language=en" "today"
Github.getRepoInfo

let themeSwitch = document.querySelector(""".theme-slider-label input[type="checkbox"]""") :?> Browser.Types.HTMLLabelElement
themeSwitch.addEventListener ("change", Utils.switchTheme)

let myButton = document.querySelector(".get-fact") :?> Browser.Types.HTMLButtonElement
myButton.onclick <- fun _ ->
    Facts.getFact "https://uselessfacts.jsph.pl/random.json?language=en" "random"


