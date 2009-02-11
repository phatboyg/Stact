namespace Magnum.ObjectExtensions
{
    using System;

    public static class EventHandlerExtensions
    {
        /// <summary>
        /// thanks, http://kohari.org/2009/02/07/eventhandler-extension-method/
        /// </summary>
        public static void Raise<T>(this EventHandler<T> handler, object sender, T args)
            where T : EventArgs
        {
            EventHandler<T> evt = handler;
            if (evt != null) evt(sender, args);
        }
    }
}