module AzureFuncFsprojSupport.HttpTrigger

open System
open System.IO
open System.Threading.Tasks
open Microsoft.Extensions.Primitives
open Microsoft.AspNetCore.Mvc
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Newtonsoft.Json
open FSharp.Control.Tasks.V2

[<FunctionName("HttpTrigger")>]
let Run
    (
        [<HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)>]
        req: HttpRequest,
        log: ILogger
    ) : Task<IActionResult> = task {

    log.LogInformation("F# HTTP trigger function processed a request.")

    let name = StringValues.op_Implicit req.Query.["name"] : string

    use streamReader = new StreamReader(req.Body)
    let! requestBody = streamReader.ReadToEndAsync()
    let name =
        if String.IsNullOrEmpty requestBody then
            name
        else
            let data = JsonConvert.DeserializeObject<{| name: string |}>(requestBody)
            if isNull name then
                data.name
            else
                name

    return
        if name <> null then
            OkObjectResult(sprintf "Hello, %s" name) :> IActionResult
        else
            BadRequestObjectResult("Please pass a name on the query string or in the request body") :> IActionResult
}
