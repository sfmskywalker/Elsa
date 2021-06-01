using Elsa.Activities.File.Bookmarks;
using Elsa.Activities.File.Models;
using Elsa.Dispatch;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Elsa.Activities.File.Extensions
{
    public static class WorkflowRunnerExtensions
    {
        const string _activityType = nameof(WatchDirectory);

        public static async Task TriggerFileSystemChangedWorkflowsAsync(this IWorkflowDispatcher workflowDispatcher,
            string directory,
            string pattern,
            WatcherChangeTypes changeType,
            string fileName,
            string fullPath,
            CancellationToken cancellationToken = default)
        {
            var input = new FileSystemChanged(changeType, directory, fileName, fullPath)
            {
                ChangeType = changeType,
                Directory = directory,
                FileName = fileName,
                FullPath = fullPath,
                Pattern = pattern,
                TimeStamp = DateTime.Now
            };
            var bookmark = new FileSystemChangedBookmark(changeType, directory, pattern);
            var trigger = new FileSystemChangedBookmark(changeType, directory, pattern);
            
            await workflowDispatcher.DispatchAsync(new TriggerWorkflowsRequest(_activityType, bookmark, trigger, input), cancellationToken);
        }
    }
}