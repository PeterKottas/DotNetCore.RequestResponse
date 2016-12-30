# DotNetCore.RequestResponse

With this lib, it probably makes sense to first explain *WHY* you need and only then start discussing how we solve it.
The potential "customer" of this lib will be a person working with horizontaly scalable c# architecture although I think it brings some value to all of you out there.

## Why

Let me give you an example. You have an architecture built around microservices. You push your request to backend, this is translated through a proxy to Messaging Queue and then to microservice. One or more. Along the way, there's a problem somewhere. You're smart so you have logs but the thing is, it's almost impossible to corelate requests and responses. The problem happend in one service, but you don't know where the request come from, possibly where it was going and that makes figuring out problems very hard. 

Imagine you have 5 config manager services running. One is missconfigured for some reason. The problem only shows way later in the chain. *HOW* would you know where it came from? 

## How

Enter Correlation IDs. I am not saying this is something new. The basic idea that was around forever I guess is to assign an ID to the whole request response chain. So you would create a base class, put ID in it. Generate it once and then pass it along. Right?

Well ... Imagine how much code that is. Every time you need to do it and you can't *forget*. Ensure that. I dare. Therefore we need a framework to make this possible. And that's where this library comes in.

## Bonus

You guys who came here are probably already thinking about it but I'll say it anyways. App dynamics ring a bell? What this lib provides is basically very very poor men's solution to simmilar problems AD is tackling so well. You'd just need a loger and some frontend and you know what's going on in your lib. Easy as that :)

## Installation

Using nuget:
**Install-Package PeterKottas.DotNetCore.RequestResponse**

## Usage

1. Create .NETCore console app with a project.json simmilar to this:
	
	```cs
	{
		"version": "1.0.0-*",
		"buildOptions": {
			"emitEntryPoint": true
		},
		"frameworks": {
			"netcoreapp1.1": {
				"dependencies": {
					"Microsoft.NETCore.App": {
						"version": "1.1.0"//Optionally add "type": "platform" if you don't want self contained app
					}
				},
				"imports": "dnxcore50"
			}
		},
		"runtimes": { //Optionally add runtimes that you want to support
			"win81-x64": {}
		}
	}
	```
2. Create a few responses and requests:
	```cs
	// For backend api
	public class IsUsernameAvaliableWebRequestDTO: CustomRequestDTO
  {
		public string Username { get; set;}
  }
	public class IsUsernameAvaliableWebResponseDTO: CustomResponseDTO
  {
		public bool IsAvaliable { get; set;}
  }
	// Another layer of requests/responses for the microservices layer to ensure separation
	public class IsUsernameAvaliableRequestDTO: CustomRequestDTO
  {
		public string Username { get; set;}
  }
	public class IsUsernameAvaliableResponseDTO: CustomResponseDTO
  {
		public bool IsAvaliable { get; set;}
  }
	```
2. Now we can go to our Program.cs and look at couple of interesting methods first let's look at *constructor*:
	```cs
	var req = new IsUsernameAvaliableWebRequestDTO()
  {
		Username = "Peter"
  };
	```
	It's nothing special really but what's important happens inside. This is the *ONLY* place where you use constructor. The reason for that is constructor intializes data like Corelation ID and a few more. You therefore only want to call it once (or let ASP.net Core take care of it when used as Request in an action) 
3. Let's look at what got initiliazed:
	```cs
	Console.WriteLine("Created first request\nId:{0}\nTimestamp:{1}\nDepth:{2}\nTimeTakenGlobal:{3}\nTimeTakenLocal:{4}\nJourney:{5}\nValue:{6}\n\n", 
    req.OperationId, 
    req.EnterTimestamp, 
    req.Depth, 
    req.TimeTakenGlobal,
    req.TimeTakenLocal,
    string.Join("\n", req.GetJourney()),
    req.Username);
	```
..* *OperationId* - Corelation id unique for the chain of requests/responses
..* *EnterTimestamp* - Timestamp of when the request was created
..* *Depth* - Depth of the request response chain
..* *TimeTakenGlobal* - Total time from the first request
..* *TimeTakenLocal* - Time from the last request/response to the current one
..* *GetJourney()* - Returns a list of requests/responses. Basically whole chain.
..* *Username* - This comes from our concrete DTO
4. Now let's create another request out of this one (this can look simmilar to mapping)
	```cs
	var plugReq = req.GetRequest<IsUsernameAvaliableRequestDTO>(operationInner =>
  {
    operationInner.Username = req.Username;
    return operationInner;
  });
	Console.WriteLine("Created first request\nId:{0}\nTimestamp:{1}\nDepth:{2}\nTimeTakenGlobal:{3}\nTimeTakenLocal:{4}\nJourney:{5}\nValue:{6}\n\n", 
    req.OperationId, //Stays the same
    req.EnterTimestamp, //Stays the same
    req.Depth, //Now 2 used to be 1
    req.TimeTakenGlobal, //Time from EnterTimestamp
    req.TimeTakenLocal, // Time from LastRequest
    string.Join("\n", req.GetJourney()), //Curently contains 2 requests
    req.Username);
	```
	Notice how we didn't use the constructor this time. We used generic method GetRequest (we would use GetResponse equivalent for a response) providing generic parameter that defines that actual type of the request we want to create. IsUsernameAvaliableRequestDTO in this case. 
5. Journey
	You might want to opt out/in from creating Journey which is chain of requests responses. You can enable disable this feature by calling:
	```cs
  req.LogJourney(); // enables
	req.LogJourney(false); // disables
6. Check out the full example in this [repo](https://github.com/PeterKottas/DotNetCore.RequestResponse/blob/master/Source/PeterKottas.DotNetCore.RequestResponse.Example/Program.cs)
## Advanced usage
In the previous example, we looked at the most basic setup. But what if you want to create your own base classes and override what happens in them? Let's do just that with an example scenario:
Let's say we want to add a property *Counter* sole purpose of which will be to responses/requests we got so far:
1. Start by creating base operation, request and response. Operation is the base for both request and response allowing for implementation shared between request and response
	```cs
	public abstract class CustomOperationDTO : BaseOperationDTO<CustomRequestDTO, CustomResponseDTO, CustomOperationDTO>
  {
  }
	public abstract class CustomRequestDTO : CustomOperationDTO
  {
  }
	public abstract class CustomResponseDTO : CustomOperationDTO
  {
  }
	```
2. Add required property:
	```cs
	public abstract class CustomOperationDTO : BaseOperationDTO<CustomRequestDTO, CustomResponseDTO, CustomOperationDTO>
  {
		public int Counter {get;set;}
  }
	```
2. Override a method responsible for creating a new operation instance:
	```cs
	protected override sealed BASE_CLASS GetOperationCustom<BASE_CLASS>(BASE_CLASS operation)
  {
    operation.Counter = this.Counter + 1;
    return operation;
  }
	```
3. And we're done. 
	Notice how we first create a new Conrete class out of the generic parameter. Then perform any logic while generics ensure "type safety". Full example [here](https://github.com/PeterKottas/DotNetCore.RequestResponse/blob/master/Source/PeterKottas.DotNetCore.RequestResponse.Example/DTO/CustomBaseOperationDTO.cs)

## Contributing

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request :D

## License

MIT 
