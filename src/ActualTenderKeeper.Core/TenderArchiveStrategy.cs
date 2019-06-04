using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ActualTenderKeeper.Abstract;
using Infrastructure.Abstract.Logging;

using static ActualTenderKeeper.Core.TenderElasticQuery;

namespace ActualTenderKeeper.Core
{
    public sealed class TenderArchiveStrategy : ITenderArchiveStrategy
    {
        private readonly INotActualTendersArchiver _archiver;
        private readonly IElasticsearchOptions _options;
        private readonly ILog _log;

        public TenderArchiveStrategy(INotActualTendersArchiver archiver,
            IElasticsearchOptions options,
            ILog log)
        {
            _archiver = archiver ?? throw new ArgumentNullException(nameof(archiver));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }
        
        #region ITenderArchiveStrategy

        public async Task ArchiveNotActualTenders(CancellationToken ct = default(CancellationToken))
        {
            _log.Info("Start reindexing of not actual tenders " +
                      $"from index {_options.ActualTenderIndexName} to index {_options.ArchiveTenderIndexName}");
            
            var r = await _archiver.ReindexNotActualTenders(NotActualTendersQueryJson, ct);
            
            _log.Info("Reindexing of not actual tenders completed");
            _log.Info($"It's took {r.SpentTime.TotalMilliseconds} milliseconds");
            _log.Info($"Reindexed {r.NumberOfProcessedItems} not actual tenders");
            if (r.Failures?.Count() > 0)
            {
                _log.Info("Some failures occured:");
                _log.Info($"{string.Join(Environment.NewLine, r.Failures)}");     
            }
            
            _log.Info("Start deletion of not actual tenders " +
                      $"from index {_options.ActualTenderIndexName}");

            r = await _archiver.DeleteNotActualTenders(NotActualTendersQueryJson, ct);
            
            _log.Info("Deletion of not actual tenders completed");
            _log.Info($"It's took {r.SpentTime.TotalMilliseconds} milliseconds");
            _log.Info($"Deleted {r.NumberOfProcessedItems} not actual tenders");
            if (r.Failures?.Count() > 0)
            {
                _log.Info("Some failures occured:");
                _log.Info($"{string.Join(Environment.NewLine, r.Failures)}");     
            }
        }

        #endregion
    }
}