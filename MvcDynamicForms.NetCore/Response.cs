﻿namespace MvcDynamicForms.NetCore
{
    /// <summary>
    /// Class that represents the end user's response to an InputField object.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// The response title of the InputField object.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The user's response.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The key of the InputField object.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The tag of the InputField object
        /// </summary>
        public string Tag { get; set; }
    }
}