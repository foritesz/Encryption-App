namespace encrypt2

open System
open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html
open WebSharper.Forms

[<JavaScript>]
module Client =

    // Function to shuffle an list
    let shuffle (list: 'a list) : 'a list =
        let rng = Random()
        let array = List.toArray list
        for i in 0 .. array.Length - 1 do
            let j = rng.Next(array.Length)
            // Swap elements
            let temp = array.[i]
            array.[i] <- array.[j]
            array.[j] <- temp
        array |> List.ofArray

    // Initialize characters
    let mutable charsList: char list =
        [for c in 32 .. 126 -> char c]

    // Create a key list by shuffling charsList
    let mutable keyList = shuffle charsList

    // Function to display the encryption key
    let displayKey () : string =
        keyList
        |> List.map string
        |> String.concat ""

    // Function to save the encryption key
    let saveKey () : string =
        displayKey()

    // Function to load an encryption key
    let loadKey (key: string) =
        key
        |> Seq.map char
        |> List.ofSeq
        |> List.mapi (fun i c -> (charsList.[i], c))
        |> List.fold (fun (cl, kl) (c, k) -> (cl @ [c], kl @ [k])) ([], [])
        |> fun (cl, kl) -> (charsList <- cl; keyList <- kl)

    // Encrypt function using 
    let encrypt (plainText: string): string =
        plainText
        |> Seq.map (fun letter ->
            let index = List.findIndex ((=) letter) charsList
            keyList.[index])
        |> Seq.toArray
        |> Array.map string
        |> String.concat ""

    // Decrypt function using
    let decrypt (cipherText: string): string =
        cipherText
        |> Seq.map (fun letter ->
            let index = List.findIndex ((=) letter) keyList
            charsList.[index])
        |> Seq.toArray
        |> Array.map string
        |> String.concat ""

    // Reverse encryption function
    let reverseEncrypt (plainText: string): string =
        plainText
        |> Seq.rev
        |> Seq.toArray
        |> System.String
        |> encrypt

    // Helper functions to convert characters to lower and upper case using JavaScript
    [<Inline "String.fromCharCode($letter).toLowerCase().charCodeAt(0)">]
    let toLower (letter: char) : char = X<char>

    [<Inline "String.fromCharCode($letter).toUpperCase().charCodeAt(0)">]
    let toUpper (letter: char) : char = X<char>

    // Case-sensitive encryption function
    let caseSensitiveEncrypt (plainText: string): string =
        plainText
        |> Seq.map (fun letter ->
            if Char.IsUpper letter then
                let lower = toLower letter
                let index = List.findIndex ((=) lower) charsList
                toUpper keyList.[index]
            else
                let index = List.findIndex ((=) letter) charsList
                keyList.[index])
        |> Seq.toArray
        |> Array.map string
        |> String.concat ""

    [<SPAEntryPoint>]
    let Main () =
        let plainText = Var.Create ""
        let encryptedText = Var.Create ""
        let decryptedText = Var.Create ""
        let encryptionKey = Var.Create (displayKey())

        div [attr.``class`` "auth-container"] [
            h1 [] [text "Encryption App"]
            div [attr.``class`` "form-group"] [
                h3 [] [text "Encrypt a Message"]
                Doc.Input [attr.``class`` "form-control"] plainText
                button [attr.``class`` "btn btn-default" ; on.click (fun _ _ -> encryptedText.Value <- encrypt plainText.Value)] [text "Encrypt"]
                button [attr.``class`` "btn btn-default" ; on.click (fun _ _ -> encryptedText.Value <- reverseEncrypt plainText.Value)] [text "Reverse Encrypt"]
                button [attr.``class`` "btn btn-default" ; on.click (fun _ _ -> encryptedText.Value <- caseSensitiveEncrypt plainText.Value)] [text "Case-Sensitive Encrypt"]
            ]
            div [attr.``class`` "form-group"] [
                h3 [] [text "Encrypted Message"]
                Doc.Input [attr.``class`` "form-control" ; attr.readonly "readonly"] encryptedText
            ]
            div [attr.``class`` "form-group"] [
                h3 [] [text "Decrypt the Message"]
                Doc.Input [attr.``class`` "form-control"] encryptedText
                button [attr.``class`` "btn btn-default" ; on.click (fun _ _ -> decryptedText.Value <- decrypt encryptedText.Value)] [text "Decrypt"]
            ]
            div [attr.``class`` "form-group"] [
                h3 [] [text "Decrypted Message"]
                Doc.Input [attr.``class`` "form-control" ; attr.readonly "readonly"] decryptedText
            ]
            div [attr.``class`` "form-group"] [
                h3 [] [text "Encryption Key"]
                Doc.Input [attr.``class`` "form-control" ; attr.readonly "readonly"] encryptionKey
                button [attr.``class`` "btn btn-default" ; on.click (fun _ _ -> encryptionKey.Value <- saveKey())] [text "Save Key"]
                button [attr.``class`` "btn btn-default" ; on.click (fun _ _ ->
                    let key = JS.Window.Prompt("Enter the encryption key:")
                    if not (String.IsNullOrWhiteSpace key) then
                        loadKey key
                        encryptionKey.Value <- displayKey()
                )] [text "Load Key"]
            ]
        ]
        |> Doc.RunById "main"
