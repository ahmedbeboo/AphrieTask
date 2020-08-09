using Entities;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AphrieTask.Manager
{
    public class LogManager
    {
        private IBaseRepository<Log> _logRepository;

        public LogManager(IBaseRepository<Log> logRepository)
        {
            _logRepository = logRepository;
        }

        public void addLog(Log log)
        {
            try
            {
                _logRepository.Insert(log);
            }
            catch { }
        }

    }
}
