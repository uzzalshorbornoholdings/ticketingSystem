using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace BitswardITSM.Core
{
    public class SlaEngine
    {
        private readonly DatabaseManager _db;

        public SlaEngine(DatabaseManager db)
        {
            _db = db;
        }

        public class SlaConfig
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int ResolutionHours { get; set; }
            public int AlertThresholdMinutes { get; set; }
        }

        /// <summary>
        /// Fetches the SLA config parameters by priority name (P1, P2, P3, P4).
        /// </summary>
        public SlaConfig GetSlaConfig(string priorityName)
        {
            string query = "SELECT id, name, resolution_hours, alert_threshold_minutes FROM slas WHERE name = @name";
            var dt = _db.ExecuteQuery(query, new MySqlParameter[] { new MySqlParameter("@name", priorityName) });

            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                return new SlaConfig
                {
                    Id = Convert.ToInt32(row["id"]),
                    Name = row["name"].ToString(),
                    ResolutionHours = Convert.ToInt32(row["resolution_hours"]),
                    AlertThresholdMinutes = Convert.ToInt32(row["alert_threshold_minutes"])
                };
            }

            // Fallback defaults if SLA table seed is missing or mismatched
            int resHours = 24;
            int alertMins = 360;
            switch (priorityName.ToUpper())
            {
                case "P1": resHours = 2; alertMins = 30; break;
                case "P2": resHours = 8; alertMins = 120; break;
                case "P3": resHours = 24; alertMins = 360; break;
                case "P4": resHours = 72; alertMins = 1440; break;
            }

            return new SlaConfig
            {
                Id = 0,
                Name = priorityName,
                ResolutionHours = resHours,
                AlertThresholdMinutes = alertMins
            };
        }

        /// <summary>
        /// Calculates the deadline based on business hours (9:00 AM - 5:00 PM, Monday to Friday).
        /// </summary>
        public DateTime CalculateDeadline(DateTime start, int resolutionHours)
        {
            double minutesToAdd = resolutionHours * 60.0;
            DateTime current = start;

            while (minutesToAdd > 0)
            {
                // 1. Skip weekends
                if (current.DayOfWeek == DayOfWeek.Saturday)
                {
                    current = current.Date.AddDays(2).AddHours(9); // Monday 9:00 AM
                    continue;
                }
                if (current.DayOfWeek == DayOfWeek.Sunday)
                {
                    current = current.Date.AddDays(1).AddHours(9); // Monday 9:00 AM
                    continue;
                }

                // 2. Adjust off-hours
                if (current.Hour < 9)
                {
                    current = current.Date.AddHours(9); // Fast-forward to start of workday 9:00 AM
                }
                else if (current.Hour >= 17)
                {
                    current = current.Date.AddDays(1).AddHours(9); // Fast-forward to 9:00 AM next day
                    continue;
                }

                // 3. Add time inside current business day
                DateTime endOfBusinessDay = current.Date.AddHours(17);
                double minutesRemainingInDay = (endOfBusinessDay - current).TotalMinutes;

                if (minutesToAdd <= minutesRemainingInDay)
                {
                    current = current.AddMinutes(minutesToAdd);
                    minutesToAdd = 0;
                }
                else
                {
                    minutesToAdd -= minutesRemainingInDay;
                    current = current.Date.AddDays(1).AddHours(9); // Hop to next business day at 9:00 AM
                }
            }

            return current;
        }

        /// <summary>
        /// Checks if a ticket has breached its SLA limit.
        /// </summary>
        public bool IsBreached(DateTime createdAt, DateTime? resolvedAt, string priority)
        {
            var config = GetSlaConfig(priority);
            DateTime deadline = CalculateDeadline(createdAt, config.ResolutionHours);
            DateTime checkTime = resolvedAt ?? DateTime.Now;
            return checkTime > deadline;
        }

        /// <summary>
        /// Returns how many minutes are remaining until the SLA is breached,
        /// or a negative count if the SLA was already breached.
        /// </summary>
        public double GetMinutesRemaining(DateTime createdAt, string priority)
        {
            var config = GetSlaConfig(priority);
            DateTime deadline = CalculateDeadline(createdAt, config.ResolutionHours);
            return (deadline - DateTime.Now).TotalMinutes;
        }

        /// <summary>
        /// Indicates if a ticket has entered the warnings alert time frame.
        /// </summary>
        public bool IsNearBreach(DateTime createdAt, string priority)
        {
            var config = GetSlaConfig(priority);
            DateTime deadline = CalculateDeadline(createdAt, config.ResolutionHours);
            double remainingMinutes = (deadline - DateTime.Now).TotalMinutes;
            return remainingMinutes > 0 && remainingMinutes <= config.AlertThresholdMinutes;
        }
    }
}
