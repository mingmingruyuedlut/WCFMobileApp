
using System;

namespace Interactive.DBManager.Entity
{
    public class TokenEntity
    {
        public int Id { get; set; }

        public string UEmail { get; set; }

        public DateTime StartTime { get; set; }

        public string Value { get; set; }

        public bool IsTokenValid { get; set; }

        public bool IsExist { get; set; }

        public bool IsTokenExist { get; set; }

        public bool IsTokenExpired { get; set; }
    }
}
