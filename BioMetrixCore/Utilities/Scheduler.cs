using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore.Utilities
{
    public class Scheduler
    {
        private StdSchedulerFactory factory = new StdSchedulerFactory();
        private IScheduler scheduler;

        public async Task Start()
        {
            scheduler = await factory.GetScheduler();
            await scheduler.Start();

            await ScheduleSyncLogJob(Session.UserOption.SyncOption);
            //await ScheduleBackupDBJob(Session.UserOption.BackupOption);
        }

        private async Task ScheduleSyncLogJob(SyncOption option)
        {
            //if (option != null && Session.AccessToken != null)
            if (option != null || Session.AccessToken != null)
            {
                foreach (var time in option.SyncTimes)
                {
                    var job = JobBuilder.Create<SyncLogJob>()
                                        .WithIdentity($"SyncLog_{time.SyncTime}")
                                        .Build();
                    // ntgiang2 16.3.2021
                    // kiểm tra có job trong schedule trùng không, nếu có thì xóa
                    if (scheduler.CheckExists(job.Key).Result)
                    {
                        scheduler.DeleteJob(job.Key).Wait();
                    }
                    var trigger = TriggerBuilder.Create()
                                                .WithIdentity($"SyncLogTime_{time.SyncTime}")
                                                .WithDailyTimeIntervalSchedule(x => x
                                                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(time.Hour, time.Minute))
                                                    .EndingDailyAfterCount(1)
                                                    .OnEveryDay())
                                                .Build();
                    await scheduler.ScheduleJob(job, trigger);
                }
            }
        }

        //private async Task ScheduleBackupDBJob(BackupOption option)
        //{
        // if (option != null)
        // {
        // var job = JobBuilder.Create<BackupDBJob>()
        // .WithIdentity($"BackupDB")
        // .UsingJobData("Option", Converter.JsonSerialize(option))
        // .Build();
        // var trigger = TriggerBuilder.Create()
        // .WithIdentity($"BackupDBTime")
        // .WithDailyTimeIntervalSchedule(x => x
        // .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(option.BackupTimeHour, option.BackupTimeMinute))
        // .EndingDailyAfterCount(1)
        // .OnEveryDay())
        // .Build();
        // await scheduler.ScheduleJob(job, trigger);
        // }
        //}

        public async Task Shutdown()
        {
            if (scheduler != null && scheduler.IsStarted)
            {
                await scheduler.Shutdown(false);
            }
        }
    }

    public class SyncLogJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Logger.LogError("[Execute SyncLogJob]");
            return Task.Run(() =>
            {
                // Thực hiện chạy job
            });
        }
    }

    //public class BackupDBJob : IJob
    //{
    // public Task Execute(IJobExecutionContext context)
    // {
    // return Task.Run(() =>
    // {
    // var option = Converter.JsonDeserialize<BackupOption>((string)context.MergedJobDataMap["Option"]);
    // if (option.IsAutoBackup)
    // {
    // if (Directory.Exists(option.BackupLocation))
    // {
    // var backupFileName = Path.Combine(option.BackupLocation, DBManager.GetDBNameBackup(Session.Tenant.TenantCode));
    // var backuped = DBManager.BackupDB(DBManager.GetDBPath(Session.Tenant.TenantCode), backupFileName);
    // if (backuped)
    // {
    // var blSystem = new BLSystem();
    // Session.UserOption.BackupOption.LastBackupDate = DateTime.Now;
    // blSystem.SaveUserOption();
    // }
    // }
    // }
    // });
    // }
    //}
}
