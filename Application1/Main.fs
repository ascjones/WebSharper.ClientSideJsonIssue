namespace Application1

open WebSharper
open WebSharper.Sitelets

type Article = { title: string; body: string }

type EndPoints =
    | [<EndPoint "POST /article"; Json "article">]
        PostArticle of article: Article

module Site =

    [<Website>]
    let app = Sitelet.Infer <| fun cxt endpoint ->
        match endpoint with
        | PostArticle article ->
            let articleId = 1
            Content.Json(fun _ -> articleId)

[<JavaScript>]
module Client =
    open WebSharper.JavaScript
    open WebSharper.JQuery

    /// General function to send an AJAX request with a body.
    let Ajax (httpMethod: string) (url: string) (serializedData: string) : Async<string> =
        Async.FromContinuations <| fun (ok, ko, _) ->
            JQuery.Ajax(
                JQuery.AjaxSettings(
                    Url = url,
                    Type = As<JQuery.RequestType> httpMethod,
                    ContentType = "application/json",
                    DataType = JQuery.DataType.Text,
                    Data = serializedData,
                    Success = (fun (result, _, _) -> ok (result :?> string)),
                    Error = (fun (jqXHR, _, _) -> ko (System.Exception(jqXHR.ResponseText)))))
            |> ignore

    /// Use Json.Serialize and Deserialize to send and receive data to and from the server.
    let PostBlogArticle (article: Article) : Async<int> =
        async { let! response = Ajax "POST" "/article" (Json.Serialize article)
                return Json.Deserialize<int> response }

module SelfHostedServer =

    open global.Owin
    open Microsoft.Owin.Hosting
    open Microsoft.Owin.StaticFiles
    open Microsoft.Owin.FileSystems
    open WebSharper.Owin

    [<EntryPoint>]
    let Main = function
        | [| rootDirectory; url |] ->
            use _server = WebApp.Start(url, fun appB ->
                appB.UseStaticFiles(
                        StaticFileOptions(
                            FileSystem = PhysicalFileSystem(rootDirectory)))
                    .UseSitelet(rootDirectory, Site.app)
                |> ignore)
            stdout.WriteLine("Serving {0}", url)
            stdin.ReadLine() |> ignore
            0
        | _ ->
            eprintfn "Usage: Application1 ROOT_DIRECTORY URL"
            1
