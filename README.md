# tech-assessment

## How to run the app

Run

```
Docker compose up
```

Then visit [local](http://localhost:8080/)

All the necessary migrations and seeders on start



## IA Usage
The project's core structure was built manually, with all architectural decisions made by me as well as the entities and the relation between them on the back and the views with the header on the front.

From that point on, I would simply provide the request DTO, the endpoint responsible for receiving it, and the signature of the handler that would process it.

I would then prompt Opus 5.5, specifying which endpoint needed to be implemented and which Vue component would consume it.

Afterward, I would review the entire implementation and create the tests covering the edge cases I had identified through prompts.

I didn't use any custom skills or provide an AGENTS.md file. I believe frontier models are capable enough to understand and follow the architectural decisions already made.
