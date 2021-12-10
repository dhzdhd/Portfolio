module E

open Browser.Dom
open Fetch
open Thoth.Json

module private facts =
    let update text =
        let todayFact = document.querySelector(".today-fact-text") :?> Browser.Types.HTMLHeadingElement
        todayFact.innerText <- text
        ()

    let getTodayFact (url:string) =
        fetch url []
        |> Promise.bind (fun res -> res.text())
        |> Promise.map (fun txt -> 
            let decoded = Decode.Auto.fromString<{|Text: string|}> (txt, caseStrategy = CamelCase)
            match decoded with
            | Ok resp ->  
                update resp.Text
            | Error decodingError -> 
                printfn "Error"
        )
        |> ignore
    ()

let myButton = document.querySelector(".down-button") :?> Browser.Types.HTMLButtonElement


facts.getTodayFact "https://uselessfacts.jsph.pl/today.json?language=en"
// myButton.onclick <- fun _ ->
//     repositories.getBio ()


