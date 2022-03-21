﻿using Elsa.Jobs.Contracts;
using Elsa.Modules.Hangfire.Jobs;
using Hangfire;
using Hangfire.States;
using HangfireJob = Hangfire.Common.Job;

namespace Elsa.Modules.Hangfire.Services;

public class HangfireJobQueue : IJobQueue
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public HangfireJobQueue(IBackgroundJobClient backgroundJobClient, IJobRunner jobRunner)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    public Task SubmitJobAsync(IJob job, string? queueName = default, CancellationToken cancellationToken = default)
    {
        var hangfireJob = HangfireJob.FromExpression<RunElsaJob>(x => x.RunAsync(job, CancellationToken.None));
        _backgroundJobClient.Create(hangfireJob, new EnqueuedState(queueName ?? "default"));
        
        return Task.CompletedTask;
    }
}