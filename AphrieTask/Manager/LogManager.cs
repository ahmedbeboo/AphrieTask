using Entities;
using Entities.Enums;
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

        public Log GetLog(LogLevel logLevel,string MSG,string requestInfo,string responseInfo,string userInfo)
        {
            try
            {
                Log log = new Log();
                log.CreatedDate = DateTime.Now;
                log.Level = logLevel;
                log.requestInfo = requestInfo;
                log.responseInfo = responseInfo;
                log.userInfo = userInfo;

                return log;
            }
            catch
            {
                return null;
            }
        }
    }
}
