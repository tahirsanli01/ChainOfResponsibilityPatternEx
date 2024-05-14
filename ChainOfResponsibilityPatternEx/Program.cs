using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace ChainOfResponsibilityPatternEx
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var authenticationHandler = new GrayScaleHandler();
            //var authorizationHandler = new MonoChromeHandler();
            //var validationHandler = new ValidationHandler();

            //authenticationHandler.SetNextHandler(authorizationHandler);
            //authorizationHandler.SetNextHandler(validationHandler);



            //authenticationHandler.InitializeFirstHandler(new Bitmap(""));
            //var result = authenticationHandler.HandleRequest();

            // JSON'dan veriyi oku
            string json = File.ReadAllText("C:\\Users\\sanli\\source\\repos\\WORKING\\ChainOfResponsibilityPatternEx\\ChainOfResponsibilityPatternEx\\json1.json");
            var handlerConfigs = JsonConvert.DeserializeObject<List<HandlerConfig>>(json);

            // Handler'ları oluştur ve zincirle
            IRequestHandler firstHandler = null;
            IRequestHandler prevHandler = null;
            foreach (var config in handlerConfigs)
            {
                var handler = CreateHandler(config.HandlerType);
                handler.InitializeFirstHandler(config.Data);
                if (prevHandler != null)
                {
                    prevHandler.SetNextHandler(handler);
                }
                else
                {
                    firstHandler = handler;
                }
                prevHandler = handler;
            }

            // İstekleri işle
            firstHandler.HandleRequest();
            Console.ReadLine();
        }


        private static IRequestHandler CreateHandler(string handlerType)
        {
            // "ChainOfResponsibilityPatternEx" isim alanını içeren derleme yükleniyor
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Tip adı "handlerType" olan sınıfı ara
            Type type = assembly.GetTypes().FirstOrDefault(t => t.Name == handlerType);

            if (type != null && typeof(IRequestHandler).IsAssignableFrom(type))
            {
                // Tip IRequestHandler arayüzünü uyguluyorsa bir örnek oluştur
                return (IRequestHandler)Activator.CreateInstance(type);
            }
            else
            {
                // Belirtilen türde bir sınıf bulunamadı veya IRequestHandler arayüzünü uygulamıyorsa null döndür
                return null;
            }
        }
    }



    public class GrayScaleHandler : RequestHandlerBase
    {
        [Config("GrayScaleHandlerData")]
        protected override void ProcessRequest(object data)
        {
            Console.WriteLine("GrayScale handler processing request");
            PutNextHandlerData("AuthorizationHandlerData");
        }
    }


    public class MonoChromeHandler : RequestHandlerBase
    {
        [Config("MonoChromeHandlerData")]
        protected override void ProcessRequest(object data)
        {
            Console.WriteLine("MonoChrome handler processing request");
            PutNextHandlerData("ValidationHandlerData");
        }
    }

    public class ValidationHandler : RequestHandlerBase
    {
        [Config("ValidationHandlerData")]
        protected override void ProcessRequest(object data)
        {
            Console.WriteLine("Validation handler processing request");
            PutNextHandlerData("Finish");
        }
    }


    internal class HandlerConfig
    {
        public string HandlerType { get; set; }
        public object Data { get; set; }
    }




    #region ConfigClass
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class Config : Attribute
    {
        public string _name;
        public Config(string name)
        {
            _name = name;
        }
    }

    public interface IRequestHandler
    {
        void SetNextHandler(IRequestHandler handler);
        object HandleRequest();
        void InitializeFirstHandler(object value);
    }

    public abstract class RequestHandlerBase : IRequestHandler
    {
        private static Dictionary<string, object> _data = new Dictionary<string, object>();
        private IRequestHandler _nextHandler;

        public void InitializeFirstHandler(object value)
        {
            var handlerName = this.GetType().Name;
            _data[$"{handlerName}Data"] = value;
        }

        public void SetNextHandler(IRequestHandler handler)
        {
            _nextHandler = handler;
        }

        private void PutData(object value)
        {
            string handlerName = "";
            if (_nextHandler != null)
            {
                handlerName = _nextHandler.GetType().Name;
            }
            else
            {
                handlerName = "Finish";
            }

            _data[$"{handlerName}Data"] = value;
        }

        public void PutNextHandlerData(object value)
        {
            PutData(value);
        }

        private object GetData(string name)
        {
            return _data[name];
        }

        public virtual object HandleRequest()
        {
            MethodInfo methodNextAttribute = this.GetType().GetRuntimeMethods().ToList()[0];
            var methodAttribute = methodNextAttribute.GetCustomAttribute<Config>();

            ProcessRequest(GetData(methodAttribute._name));

            if (_nextHandler != null)
            {
                _nextHandler.HandleRequest();
            }

            return GetData("FinishData");
        }

        protected abstract void ProcessRequest(object name);
    }
    #endregion
}













//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using Newtonsoft.Json;

//namespace ChainOfResponsibilityPatternEx
//{
//    internal class Program
//    {
//        static void Main(string[] args)
//        {
//            // JSON'dan veriyi oku
//            string json = File.ReadAllText("C:\\Users\\sanli\\source\\repos\\WORKING\\ChainOfResponsibilityPatternEx\\ChainOfResponsibilityPatternEx\\json1.json");
//            var handlerConfigs = JsonConvert.DeserializeObject<List<HandlerConfig>>(json);

//            // Handler'ları oluştur ve zincirle
//            IRequestHandler firstHandler = null;
//            IRequestHandler prevHandler = null;
//            foreach (var config in handlerConfigs)
//            {
//                var handler = CreateHandler(config.HandlerType);
//                handler.InitializeFirstHandler(config.Data);
//                if (prevHandler != null)
//                {
//                    prevHandler.SetNextHandler(handler);
//                }
//                else
//                {
//                    firstHandler = handler;
//                }
//                prevHandler = handler;
//            }

//            // İstekleri işle
//            var requestProcessor = new RequestProcessor(firstHandler);
//            requestProcessor.ProcessRequest("Some request data");

//            Console.ReadLine();
//        }

//        private static IRequestHandler CreateHandler(string handlerType)
//        {
//            // "ChainOfResponsibilityPatternEx" isim alanını içeren derleme yükleniyor
//            Assembly assembly = Assembly.GetExecutingAssembly();

//            // Tip adı "handlerType" olan sınıfı ara
//            Type type = assembly.GetTypes().FirstOrDefault(t => t.Name == handlerType);

//            if (type != null && typeof(IRequestHandler).IsAssignableFrom(type))
//            {
//                // Tip IRequestHandler arayüzünü uyguluyorsa bir örnek oluştur
//                return (IRequestHandler)Activator.CreateInstance(type);
//            }
//            else
//            {
//                // Belirtilen türde bir sınıf bulunamadı veya IRequestHandler arayüzünü uygulamıyorsa null döndür
//                return null;
//            }
//        }
//    }




//    public class GrayScaleHandler : ConcreteHandler
//    {
//        [Config("GrayScaleHandlerData")]
//        protected override void ProcessRequest(object data)
//        {
//            PutNextHandlerData("AuthorizationHandlerData");
//        }
//    }


//    public class MonoChromeHandler : ConcreteHandler
//    {
//        [Config("MonoChromeHandlerData")]
//        protected override void ProcessRequest(object data)
//        {
//            Console.WriteLine("MonoChrome handler processing request");
//            PutNextHandlerData("ValidationHandlerData");
//        }
//    }

//    public class ValidationHandler : ConcreteHandler
//    {
//        [Config("ValidationHandlerData")]
//        protected override void ProcessRequest(object data)
//        {
//            Console.WriteLine("Validation handler processing request");
//            PutNextHandlerData("Finish");
//        }
//    }








//    internal class HandlerConfig
//    {
//        public string HandlerType { get; set; }
//        public object Data { get; set; }
//    }

//    public interface IRequestHandler
//    {
//        void InitializeFirstHandler(object data);
//        void SetNextHandler(IRequestHandler handler);
//        void HandleRequest(string requestData);
//    }

//    public class ConcreteHandler : IRequestHandler
//    {
//        private IRequestHandler _nextHandler;

//        public void InitializeFirstHandler(object data)
//        {
//            // İlk handler'ın başlatılması için gerekli işlemler
//        }

//        public void SetNextHandler(IRequestHandler handler)
//        {
//            _nextHandler = handler;
//        }

//        public void HandleRequest(string requestData)
//        {
//            // İsteği işleme süreci
//            Console.WriteLine("Handling request: " + requestData);
//            if (_nextHandler != null)
//            {
//                _nextHandler.HandleRequest(requestData);
//            }
//        }
//    }

//    internal class RequestProcessor
//    {
//        private readonly IRequestHandler _firstHandler;

//        public RequestProcessor(IRequestHandler firstHandler)
//        {
//            _firstHandler = firstHandler;
//        }

//        public void ProcessRequest(string requestData)
//        {
//            _firstHandler.HandleRequest(requestData);
//        }
//    }
//}