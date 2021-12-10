module App

open Browser.Dom
open Fetch
open Thoth.Json

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
        )
        |> ignore

Facts.getFact "https://uselessfacts.jsph.pl/today.json?language=en" "today"

let myButton = document.querySelector(".get-fact") :?> Browser.Types.HTMLButtonElement
myButton.onclick <- fun _ ->
    Facts.getFact "https://uselessfacts.jsph.pl/random.json?language=en" "random"


