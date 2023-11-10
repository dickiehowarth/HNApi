# Hacker News Api

A Restful Api to provide retrieve the details of the best stories from the Hacker News API, as determined by their score.

Retrieve all best stories:

HTTPGET: /api/hackernews/beststories

Retrieve (n: int32) best stories, using parameter count: 

e.g. count of 5:

HTTPGET: /api/hackernews/beststories?count=5

Response: 200 Ok.

The API will return an array of the best (n) stories as returned by the Hacker News API in descending order of score, in the form:

![image](https://github.com/dickiehowarth/HNApi/assets/25568061/465c2aaa-031a-4a80-9d19-b1a64df7547c)

**HackerNews  Api and Tests:**

1. Use MS Visual Studio 2022, with support for .Net 7.
2. Open solution HNApi.
3. Restore NuGet packages
4. The two projects are described below:

HNApi

To Execute: 
1. In Debug run profile https.
2. Swagger endpoint at https://localhost:7190/swagger/index.html

Description:
ASP Net Core Web Api supporting a single endpoint via a controller
HNReposiotry - repository supporing an in-memory cache of previously retrieved story details, cleared after a refresh period.
HNHttpClient Service to support httpclient calls to the hacker news site.

HNApi Tests

To Execute:
1. open Test Explplorer and run all tests

Description:
Basic Unit Tests including Mocking using NUnit and Moq:
1. HNRepository
2. InMemoryCache
3. Json Converter

Assumptions Made that the following is acceptable:

1. This is a POC and development configuration is acceptable
2. Basic unit tests are sufficient
3. Api has no security, or CORS policy
4. Api is hosted and called from localhost
5. No retry logic or general resiliancy
6. No requirement for structured logging
7. Basic in-memory cache is sufficient to support high request volume, and stale stories are acceptable during the refresh lifecycle
8. HackerNews Api is always available
  
Enhancements - given more time
1. Extend endpoint to include Filtering, preload on associated items, search
2. Introduce Poly configuration for Hacker News Api retry access
3. Consider a more robust memory cache, dependent on final environment - perhaps MS IMemoryCache.
4. Add Postman collecvtion and tests
   
After discussion if this was to be moved from a POC to production code:
Obtain requirenments for production environment
1. Secure Api depending on requirements
2. Add Production configuration annd support
....
