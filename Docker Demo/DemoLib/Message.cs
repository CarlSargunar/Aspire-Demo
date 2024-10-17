﻿namespace DemoLib
{
    public class Message
    {
        public Guid MessageId { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Name { get; set; }
        public string MessageText { get; set; }
    }
}