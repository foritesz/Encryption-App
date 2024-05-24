# Encryption-App

## Project Overview

Encryption App is a simple web application developed using F# and WebSharper that allows users to encrypt and decrypt messages using a basic substitution cipher. The application shuffles a list of characters to create an encryption key, which can then be used to encode and decode text. Additionally, the project includes features for key management and multiple encryption modes, such as reverse encryption and case-sensitive encryption.

## Motivation

The motivation behind this project is to provide a practical example of how F# and WebSharper can be used to create interactive web applications. This project demonstrates the following:

- How to use F# for client-side web development.
- Implementing a basic encryption algorithm.
- Managing state and user input in a web application.
- Extending functionality with additional features like key management and different encryption modes.

## Features

- **Encryption**: Encode messages using a shuffled character list.
- **Decryption**: Decode messages using the same character list.
- **Key Management**: Save and load encryption keys.
- **Encryption Modes**: Support for reverse encryption and case-sensitive encryption.


 ![encrypt_readme](https://github.com/foritesz/Encryption-App/assets/144954656/83a77052-322b-4b4d-bdc4-3c52d62d0eec)

try-live link: https://foritesz.github.io/Encryption-App/
  
## Requirements

- [.NET SDK](https://dotnet.microsoft.com/download) (at least version 5.0)
- [WebSharper](https://websharper.com/) (included in the project dependencies)




## How It Works

### Encryption

The `encrypt` function takes a plaintext string as input and maps each character to its corresponding character in the shuffled `keyList`.

**Code:**
```fsharp
let encrypt (plainText: string): string =
    plainText
    |> Seq.map (fun letter ->
        let index = List.findIndex ((=) letter) charsList
        keyList.[index])
    |> Seq.toArray
    |> Array.map string
    |> String.concat ""


### Decryption

The decrypt function takes a ciphertext string as input and maps each character to its corresponding character in the original charsList.

**Code:**
let decrypt (cipherText: string): string =
    cipherText
    |> Seq.map (fun letter ->
        let index = List.findIndex ((=) letter) keyList
        charsList.[index])
    |> Seq.toArray
    |> Array.map string
    |> String.concat ""

    Key Management

The key management functions (saveKey and loadKey) handle displaying, saving, and loading the encryption key.

**Code:**

let displayKey () : string =
    keyList
    |> List.map string
    |> String.concat ""

let saveKey () : string =
    displayKey()

let loadKey (key: string) =
    key
    |> Seq.map char
    |> List.ofSeq
    |> List.mapi (fun i c -> (charsList.[i], c))
    |> List.fold (fun (cl, kl) (c, k) -> (cl @ [c], kl @ [k])) ([], [])
    |> fun (cl, kl) -> (charsList <- cl; keyList <- kl)

### Encryption Modes

Different encryption modes (reverse encryption and case-sensitive encryption) provide alternative ways to encrypt the plaintext.

Reverse Encryption:


**Code:**

let reverseEncrypt (plainText: string): string =
    plainText
    |> Seq.rev
    |> Seq.toArray
    |> System.String
    |> encrypt

### Case-Sensitive Encryption:


**Code:**

[<Inline "String.fromCharCode($letter).toLowerCase().charCodeAt(0)">]
let toLower (letter: char) : char = X<char>

[<Inline "String.fromCharCode($letter).toUpperCase().charCodeAt(0)">]
let toUpper (letter: char) : char = X<char>

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
