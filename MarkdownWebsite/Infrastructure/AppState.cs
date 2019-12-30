namespace MarkdownWebsite.Infrastructure
{
    using System;
    using System.Collections.Generic;

    public class AppState
    {
        public AppState()
        {
            AvailableVerbs = new List<Type>();
        }

        public List<Type> AvailableVerbs { get; }
    }
}