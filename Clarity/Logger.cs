// ****************************************************************************
// <copyright>
// Copyright © Paul Sanders 2014
// </copyright>
// ****************************************************************************
// <author>Paul Sanders</author>
// <project>Clarity</project>
// <web>http://clarity.codeplex.com</web>
// <license>
// See license.txt in this solution
// </license>
// ****************************************************************************
using System;

namespace Clarity
{
    public class Logger
    {
        private static ILog _log = new NullLogger();
        public static void SetLogger(ILog logger)
        {
            logger.IfNullThrow("logger");
            _log = logger;
        }

        public static void Debug(string message, params object[] parameters)
        {
            _log.Debug(message, parameters);
        }

        public static void Info(string message, params object[] parameters)
        {
            _log.Info(message, parameters);
        }

        public static void Warn(string message, params object[] parameters)
        {
            _log.Warn(message, parameters);
        }

        public static void Error(string message, params object[] parameters)
        {
            _log.Error(message, parameters);
        }

        public static void LogException(Exception ex)
        {
            _log.LogException(ex);
        }
    }

    public class NullLogger : ILog
    {
        public void Debug(string message, params object[] parameters)
        {
            //do nothing
        }

        public void Info(string message, params object[] parameters)
        {
            //do nothing
        }

        public void Warn(string message, params object[] parameters)
        {
            //do nothing
        }

        public void Error(string message, params object[] parameters)
        {
            //do nothing
        }

        public void LogException(Exception ex)
        {
            //do nothing
        }
    }

    public class ConsoleLogger : ILog
    {
        public void Debug(string message, params object[] parameters)
        {
            Console.WriteLine("DEBUG: " + string.Format(message, parameters));
        }

        public void Info(string message, params object[] parameters)
        {
            Console.WriteLine("INFO: " + string.Format(message, parameters));
        }

        public void Warn(string message, params object[] parameters)
        {
            Console.WriteLine("WARN: " + string.Format(message, parameters));
        }

        public void Error(string message, params object[] parameters)
        {
            Console.WriteLine("ERROR: " + string.Format(message, parameters));
        }

        public void LogException(Exception ex)
        {
            Console.WriteLine("EXCEPTION: " + ex.Message);
        }
    }

    public class DiagnosticLogger : ILog
    {
        public void Debug(string message, params object[] parameters)
        {
            System.Diagnostics.Debug.WriteLine("DEBUG: " + string.Format(message, parameters));
        }

        public void Info(string message, params object[] parameters)
        {
            System.Diagnostics.Debug.WriteLine("INFO: " + string.Format(message, parameters));
        }

        public void Warn(string message, params object[] parameters)
        {
            System.Diagnostics.Debug.WriteLine("WARN: " + string.Format(message, parameters));
        }

        public void Error(string message, params object[] parameters)
        {
            System.Diagnostics.Debug.WriteLine("ERROR: " + string.Format(message, parameters));
        }

        public void LogException(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("EXCEPTION: " + ex.Message);
            var indent = "\t";

            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                System.Diagnostics.Debug.WriteLine(indent + "EXCEPTION: " + ex.Message);
                indent += "\t";
            }
        }
    }
}
