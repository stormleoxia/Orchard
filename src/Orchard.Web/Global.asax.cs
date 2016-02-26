using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Orchard.Environment;
using Orchard.WarmupStarter;

namespace Orchard.Web {
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication {
        private static Starter<IOrchardHost> _starter;

        public MvcApplication() {
        }

        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        }

        protected void Application_Start() {
			//this.Error += Application_Error;				
            RegisterRoutes(RouteTable.Routes);
            _starter = new Starter<IOrchardHost>(HostInitialization, HostBeginRequest, HostEndRequest);
            _starter.OnApplicationStart(this);
        }

        protected void Application_BeginRequest() {
            _starter.OnBeginRequest(this);
        }

        protected void Application_EndRequest() {
            _starter.OnEndRequest(this);
        }

        private static void HostBeginRequest(HttpApplication application, IOrchardHost host) {
            application.Context.Items["originalHttpContext"] = application.Context;
            host.BeginRequest();
        }

        private static void HostEndRequest(HttpApplication application, IOrchardHost host) {
            host.EndRequest();
        }

        private static IOrchardHost HostInitialization(HttpApplication application) {
            var host = OrchardStarter.CreateHost(MvcSingletons);

            host.Initialize();

            // initialize shells to speed up the first dynamic query
            host.BeginRequest();
            host.EndRequest();

            return host;
        }

        static void MvcSingletons(ContainerBuilder builder) {
            builder.Register(ctx => RouteTable.Routes).SingleInstance();
            builder.Register(ctx => ModelBinders.Binders).SingleInstance();
            builder.Register(ctx => ViewEngines.Engines).SingleInstance();
        }

		void Application_Error(object sender, EventArgs e)
		{
			// Code that runs when an unhandled error occurs

			// Get the exception object.
			Exception exc = Server.GetLastError();

			// Handle HTTP errors
			if (exc.GetType() == typeof(HttpException))
			{
				// The Complete Error Handling Example generates
				// some errors using URLs with "NoCatch" in them;
				// ignore these here to simulate what would happen
				// if a global.asax handler were not implemented.
				if (exc.Message.Contains("NoCatch") || exc.Message.Contains("maxUrlLength"))
					return;

				//Redirect HTTP errors to HttpError page
				Server.Transfer("HttpErrorPage.aspx");
			}

			// For other kinds of errors give the user some information
			// but stay on the default page
			Response.Write("<h2>Global Page Error</h2>\n");
			Response.Write(
				"<p>" + exc.Message + "</p>\n");
			Response.Write("Return to the <a href='Default.aspx'>" +
			     "Default Page</a>\n");

			// Log the exception and notify system operators
			ExceptionUtility.LogException(exc, "DefaultPage");
			ExceptionUtility.NotifySystemOps(exc);

			// Clear the error from the server
			Server.ClearError();
		}

    }

	// Create our own utility for exceptions 
	public sealed class ExceptionUtility
	{
		// All methods are static, so this can be private 
		private ExceptionUtility()
		{ }

		// Log an Exception 
		public static void LogException(Exception exc, string source)
		{
			// Include enterprise logic for logging exceptions 
			// Get the absolute path to the log file 
			string logFile = "App_Data/ErrorLog.txt";
			logFile = HttpContext.Current.Server.MapPath(logFile);

			// Open the log file for append and write the log
			StreamWriter sw = new StreamWriter(logFile, true);
			sw.WriteLine("********** {0} **********", DateTime.Now);
			if (exc.InnerException != null)
			{
				sw.Write("Inner Exception Type: ");
				sw.WriteLine(exc.InnerException.GetType().ToString());
				sw.Write("Inner Exception: ");
				sw.WriteLine(exc.InnerException.Message);
				sw.Write("Inner Source: ");
				sw.WriteLine(exc.InnerException.Source);
				if (exc.InnerException.StackTrace != null)
				{
					sw.WriteLine("Inner Stack Trace: ");
					sw.WriteLine(exc.InnerException.StackTrace);
				}
			}
			sw.Write("Exception Type: ");
			sw.WriteLine(exc.GetType().ToString());
			sw.WriteLine("Exception: " + exc.Message);
			sw.WriteLine("Source: " + source);
			sw.WriteLine("Stack Trace: ");
			if (exc.StackTrace != null)
			{
				sw.WriteLine(exc.StackTrace);
				sw.WriteLine();
			}
			sw.Close();
		}

		// Notify System Operators about an exception 
		public static void NotifySystemOps(Exception exc)
		{
			// Include code for notifying IT system operators
		}
	}
}
