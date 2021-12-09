module App

open Browser.Dom
// open Octokit

// open FsHttp
// open FsHttp.DslCE

// module private repositories =
//     let getBio () =
//         http {
//             POST "https://reqres.in/api/users"
//             CacheControl "no-cache"
//             body
//             json """
//             {
//                 "name": "morpheus",
//                 "job": "leader"
//             }
//             """
//         } 
//         |> Response.toJson 
//     //     let client = GitHubClient (ProductHeaderValue "dhzdhd")
//     //     let a = client.User.Get "dhzdhd"
//     //     a.Result.Bio
//     ()

// Mutable variable to count the number of times we clicked the button
let mutable count = 0

// Get a reference to our button and cast the Element to an HTMLButtonElement
let myButton = document.querySelector(".down-button") :?> Browser.Types.HTMLButtonElement


// Register our listener
myButton.onclick <- fun _ ->
    // let e = repositories.getBio ()
    // printfn $"{e}"
    ()


