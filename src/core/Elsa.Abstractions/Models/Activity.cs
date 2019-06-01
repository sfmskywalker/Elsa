﻿using System;

namespace Elsa.Models
{
    public abstract class Activity : IActivity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string TypeName => GetType().Name;
        public string Alias { get; set; }
        public ActivityMetadata Metadata { get; set; } = new ActivityMetadata();
        public ActivityDescriptor Descriptor { get; set; }
        public ActivityDesignerDescriptor Designer { get; set; }
    }
}
