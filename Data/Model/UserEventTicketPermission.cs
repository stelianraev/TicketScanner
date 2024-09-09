﻿namespace CheckIN.Data.Model
{
    public class UserEventTicketPermission
    {
        public Guid Id { get; set; }
        public Guid TicketTypeId {  get; set; }
        public TicketType TicketType { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
