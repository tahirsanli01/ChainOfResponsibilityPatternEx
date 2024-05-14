using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

            // Process the request
            authenticationHandler.HandleRequest();
        }
    }





    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class Config : Attribute
    {
        public string _name;
        public Config(string name)
        {
            _name = name;
        }
    }






    // Handler interface
    public interface IRequestHandler
    {
        void SetNextHandler(IRequestHandler handler);
        void HandleRequest();
    }

    // Abstract base class for concrete handlers
    public abstract class RequestHandlerBase : IRequestHandler
    {
        Dictionary<string, object> _data = new Dictionary<string, object>();
        private IRequestHandler _nextHandler;

        public void SetNextHandler(IRequestHandler handler)
        {
            _nextHandler = handler;
        }

        public virtual void HandleRequest()
        {
            Type methodNextAttribute = this.GetType();
            var x = methodNextAttribute.GetNestedTypes();
            var result = methodNextAttribute.GetRuntimeMethods();
            var xc = result.ToList()[0];
            var result2 = xc.GetCustomAttribute<Config>();

            ProcessRequest(_data[result2._name]);

            if (_nextHandler != null)
            {
                _nextHandler.HandleRequest();
            }
        }

        protected abstract void ProcessRequest(object name);
    }



    // Concrete handler: Authentication Handler
    public class AuthenticationHandler : RequestHandlerBase
    {
        [Config("AuthenticationHandlerData")]
        protected override void ProcessRequest(object data)
        {
            Console.WriteLine("Authentication handler processing request");
            // Perform authentication logic
        }
    }

    // Concrete handler: Authorization Handler
    public class AuthorizationHandler : RequestHandlerBase
    {
        [Config("AuthorizationHandlerData")]
        protected override void ProcessRequest(object data)
        {
            Console.WriteLine("Authorization handler processing request");
            // Perform authorization logic
        }
    }

    // Concrete handler: Validation Handler
    public class ValidationHandler : RequestHandlerBase
    {
        [Config("ValidationHandlerData")]
        protected override void ProcessRequest(object data)
        {
            Console.WriteLine("Validation handler processing request");
            // Perform validation logic
        }
    }
}
