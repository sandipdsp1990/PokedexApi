# Pokedex API

#### How to use project
* Open __Pokedex.Api.sln__ in visual studio (or in your favourite editor) in Windows,Mac or Linux
* Set __Pokedex.Api__ project as startup project in visual studio.
* Build and start the app.
* I have added Swagger page so you would see swagger page as as default.
* I have added __dockerfile__ in root of project you will see in __Solution Items__ folder in visual studio.
* To run docker file in your local machine first run to build docker image __docker build -t pokedexapi .__ in terminal or powershell in the directory where docker file is present.
* Once that is ran run this command to start container __docker run -d -p 8080:80 --name pokedexcontainer pokedexapi__ in your local machine and browse __http://localhost:8080/__ in browser you should see swagger page. you can change 8080 port to any other local port if you wish.

#### Design decisions 
* I have used TDD to test and develop code as its efficiant and less bugs and for easy refactoring of code.
* I have created seprate projects for contracts (inerfaces) to make code more loosely coupled and used DI.
* I have added swagger page so that consumer who will consume api can test and look at all endpoints.
* I have added __HttpClientFactory__ instead of normal http client to avoid socket issues and also better way to manage multiple http clients. [more info](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests).
* i have added my own mapper instead of using automapper or any other mapper. reason is its easy to refactor and test code with our own mapper.
* i have created docker file with __restore, test, publish__ and then use those build files to generate docker image. alternatively we can build outside and then just use docker to run the app but i have used this approach as its more compact and doesn't require building in different machine (like agent in azure/aws)

#### Things i would do differently for production app
* I haven't added logging in app which is essential for bug tracing lot of other cases. we can use serilog or any other logger to log.
* i haven't added integration tests to test real __PokeApi__ and __funtranslations__ which is essential to make sure when those api changes our code also changes.
* i haven't added versioning in the api but i would like to add those in the production app to manager endpoints and its version.