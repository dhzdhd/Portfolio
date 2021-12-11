module App

open Browser.Dom
open Fetch
open Thoth.Json
open Fable.SimpleJson

type GithubJsonRecord = {
    owner: string
    repo: string
    link: string
    description: string
    language: string
    stars: int
    forks: int
}

type A = {
    lst: List<string>
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
    let updateCards (ghRecord: GithubJsonRecord) = 
        
        ()

    let getRepoInfo =
        fetch "https://gh-pinned-repos-5l2i19um3.vercel.app/?username=dhzdhd" []
        |> Promise.bind (fun res -> res.text())
        |> Promise.map (fun txt -> txt.Replace ('[', ' '))
        |> Promise.map (fun txt -> txt.Replace (']', ' '))
        |> Promise.map (fun txt -> txt.Trim ())
        |> Promise.map (fun txt -> txt.Split '}')
        |> Promise.map (fun txt -> txt|> Array.map(fun ele -> ele.TrimStart ','))
        |> Promise.map (fun txt -> txt|> Array.map(fun ele -> $"{ele}}}"))
        |> Promise.map (fun lst ->
            lst |> Array.map ( fun txt -> 
                let decoded = Decode.Auto.fromString<GithubJsonRecord> (txt, caseStrategy = CamelCase)
                match decoded with
                | Ok resp -> updateCards resp
                | Error decodingError -> printfn $"{decodingError}"
            )
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


