using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainOfResponsibilityPatternEx
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Create the request handlers
            var authenticationHandler = new AuthenticationHandler();
            var authorizationHandler = new AuthorizationHandler();
            var validationHandler = new ValidationHandler();

            // Set the chain of responsibility
            authenticationHandler.SetNextHandler(authorizationHandler);
            authorizationHandler.SetNextHandler(validationHandler);

            // Create a request
            var request = new Request { Content = "Sample request" };

            // Process the request
            authenticationHandler.HandleRequest(request);
        }
    }












    // Handler interface
    public interface IRequestHandler
    {
        void SetNextHandler(IRequestHandler handler);
        void HandleRequest(Request request);
    }

    // Abstract base class for concrete handlers
    public abstract class RequestHandlerBase : IRequestHandler
    {
        private IRequestHandler _nextHandler;

        public void SetNextHandler(IRequestHandler handler)
        {
            _nextHandler = handler;
        }

        public virtual void HandleRequest(Request request)
        {
            Request req = ProcessRequest(request);

            if (_nextHandler != null)
            {
                _nextHandler.HandleRequest(req);
            }
        }

        protected abstract Request ProcessRequest(Request request);
    }



    // Concrete handler: Authentication Handler
    public class AuthenticationHandler : RequestHandlerBase
    {
        protected override Request ProcessRequest(Request request)
        {
            Console.WriteLine("Authentication handler processing request");
            // Perform authentication logic
            return request;
        }
    }

    // Concrete handler: Authorization Handler
    public class AuthorizationHandler : RequestHandlerBase
    {
        protected override Request ProcessRequest(Request request)
        {
            Console.WriteLine("Authorization handler processing request");
            // Perform authorization logic
            return request;
        }
    }

    // Concrete handler: Validation Handler
    public class ValidationHandler : RequestHandlerBase
    {
        protected override Request ProcessRequest(Request request)
        {
            Console.WriteLine("Validation handler processing request");
            // Perform validation logic
            return request;
        }
    }

    // Request class
    public class Request
    {
        public string Content { get; set; }
        // Other request properties
    }

}
