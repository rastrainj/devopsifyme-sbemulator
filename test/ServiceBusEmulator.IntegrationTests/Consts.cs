﻿namespace ServiceBusEmulator.IntegrationTests
{
    public static class Consts
    {
        public const string TestQueueName = "test-queue";
        public const string TestQueueDlqName = "test-queue/$deadletterqueue";
        public const string TestTopicName = "test-topic";
        public const string TestSubsciption1Name = "test-topic/Subscriptions/test-sub1";
        public const string TestSubsciption2Name = "test-topic/Subscriptions/test-sub2";
        public const string TestSubsciption3Name = "test-topic/Subscriptions/test-sub3";
        
        public const string QueueCollection = "Queue";
        public const string TopicCollection = "Topic";

        public const string L3 = "L3";
    }
}
