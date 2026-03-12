# User-Management

## How To Use

### Launching With Visual Studio Code
1. Open this project's folder (User-Management) in Visual Studio Code
2. Open the terminal (CTRL + `)
3. Navigate to User-Management\API\DotnetAPI (cd .\API\DotnetAPI)
4. Run the command "dotnet run"
5. Navigate to the URL "[http://localhost:5152](http://localhost:5152)"
6. Append any endpoints and terms to the URL, or try out the API with something like [Postman](https://www.postman.com/)

### Launching With Terminal
1. Navigate to this project's folder (User-Management) in your terminal
2. Navigate to API\DotnetAPI
3. Run the command "dotnet run"
4. Navigate to the URL "[http://localhost:5152](http://localhost:5152)"
5. Append any endpoints and terms to the URL, or try out the API with something like [Postman](https://www.postman.com/)

### Valid Endpoints
- [/users](http://localhost:5152/users)
  - GET all users up to 5 per page, with additional terms as URL parameters for sorting and filtering
  - POST a new user
- [/users/{id}](http://localhost:5152/users)
  - GET a specific user by its ID
  - PUT a specific user to update its details
  - DELETE a specific user

#### Url Parameters
The following fields are usable in [/users](http://localhost:5152/users) GET requests. They can be used in combination with each other.
- sortBy
  - can sort by name, email, or age
  - allows for ascending or descending order
    - [sortBy=name:asc](http://localhost:5152/users?sortBy=name:asc) and [sortBy=name:desc](http://localhost:5152/users?sortBy=name:desc) are the two ways to sort by name
- name
  - retrieves users whose names contain the given string
    - [name=John](http://localhost:5152/users?name=John) will retrieve users whose names contain "John"
  - can retrieve users whose names match the given string instead
    - [name=John:exact](http://localhost:5152/users?name=John:exact) will retrieve users whose names are "John"
- email
  - retrieves users whose emails contain the given string
    - [email=@aol](http://localhost:5152/users?email=@aol) will retrieve users whose emails contain "@aol"
  - can retrieve users whose names match the given string instead
    - [email=test@aol.com:exact](http://localhost:5152/users?email=test@aol.com:exact) will retrieve users whose emails are "test@aol.com"
    - this will result in only a single user being retrieved, since a user cannot be created or updated with an email associated with another user

- age
  - can retrieve users who are a certain age
    - [age=20](http://localhost:5152/users?age=20) will retrieve users who are 20 years old
  - can retrieve users who are within a certain age range
    - [age=[20+TO+25]](http://localhost:5152/users?age=[20+TO+25]) will retrieve users who are 20 to 25 years old

## Assumptions Made
- An in-memory database is preferred over using SQLite (based on "**in-memory database**" being bold in the instructions).
- If a user has already been created, deletion of users found to be below 18 will be handled elsewhere, so that a PUT request with the incorrect DateOfBirth will not result in accidental deletion of users.
- There are no additional limitations on email (such as ensuring that the domain is valid).
- The generated CreatedAt and UpdatedAt DateTimes and the user's Id will not need to be set or updated as part of a POST or PUT request body.
  - CreatedAt and UpdatedAt are generated at the time of the requests, and changing the user's Id could cause issues if the user is referenced elsewhere at the time of the requests.
- When using the API, __all__ data regarding a user should be returned.
- Use of the API will never result in a page number of more than 8 digits.
- Maximum page length/size should not be configurable by users of the API.
- The only error status codes that my code should create are the three listed (400, 404, 500).
- Errors that are already handled and produce a message are acceptable as-is, so long as they do not cause further issues, such as the API crashing or failing to take in and act upon subsequent requests.
  - This refers to errors such as accessing a URL without a valid endpoint, such as [http://localhost:5152/fakeEndpoint](http://localhost:5152/fakeEndpoint).

## Additional Features

### Recommended
- [x] Pagination on GET /users
- [ ] JWT Authentication 
- [ ] Unit Tests
- [x] Swagger API Documentation
- [ ] Deploy with Docker

### Others Implemented
- Filtering by age, name, and email combined
- Sorting in ascending *or* descending order
- User-friendly error messages for most BadRequests to guide them toward valid inputs
- Filtering by name and email allows for partial matches