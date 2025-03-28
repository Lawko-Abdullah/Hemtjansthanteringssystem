using System;
using System.Collections.Generic;
using System.Linq;

public class Staff
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
    public string PhotoUrl { get; set; }

    public Staff(int id, string name, string role, string photoUrl)
    {
        Id = id;
        Name = name;
        Role = role;
        PhotoUrl = photoUrl;
    }
}

public class Schedule
{
    public int UserId { get; set; }
    public int StaffId { get; set; }
    public DateTime VisitTime { get; set; }
    public string Task { get; set; }
    public string Duration { get; set; }
    public bool Confirmed { get; set; }

    public Schedule(int userId, int staffId, DateTime visitTime, string task, string duration)
    {
        UserId = userId;
        StaffId = staffId;
        VisitTime = visitTime;
        Task = task;
        Duration = duration;
        Confirmed = false;
    }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string ContactDetails { get; set; }
    public List<Schedule> Schedule { get; set; }

    public User(int id, string name, string address, string contactDetails)
    {
        Id = id;
        Name = name;
        Address = address;
        ContactDetails = contactDetails;
        Schedule = new List<Schedule>();
    }
}

public class HomeCareSystem
{
    private List<Staff> staffList;
    private List<User> userList;

    public HomeCareSystem()
    {
        staffList = new List<Staff>();
        userList = new List<User>();
    }

    public void AddStaff(Staff staff)
    {
        staffList.Add(staff);
    }

    public void AddUser(User user)
    {
        userList.Add(user);
    }

    public Staff GetStaffById(int staffId)
    {
        return staffList.FirstOrDefault(staff => staff.Id == staffId);
    }

    public List<Schedule> ViewUserSchedule(User user)
    {
        return user.Schedule;
    }

    public List<Schedule> ViewUserScheduleForDay(User user, DateTime day)
    {
        return user.Schedule.Where(s => s.VisitTime.Date == day.Date).ToList();
    }

    public string SendReminder(User user, DateTime visitTime)
    {
        var timeLeft = visitTime - DateTime.Now;
        if (timeLeft.TotalSeconds <= 3600)
        {
            return $"Reminder: You have a scheduled visit from {user.Name} at {visitTime.ToString("yyyy-MM-dd HH:mm")}.";
        }
        else
        {
            return "No reminders for now.";
        }
    }

    public string RescheduleVisit(User user, int oldStaffId, int newStaffId)
    {
        var schedule = user.Schedule.FirstOrDefault(s => s.StaffId == oldStaffId);
        if (schedule != null)
        {
            schedule.StaffId = newStaffId;
            var staff = GetStaffById(newStaffId);
            return $"Visit rescheduled: {staff.Name} will now visit you.";
        }
        return "No visit found to reschedule.";
    }

    public string ConfirmVisit(User user, DateTime visitTime)
    {
        var schedule = user.Schedule.FirstOrDefault(s => s.VisitTime == visitTime);
        if (schedule != null)
        {
            schedule.Confirmed = true;
            return $"Confirmation received: You are aware of the visit at {visitTime.ToString("yyyy-MM-dd HH:mm")}.";
        }
        return "No such visit found to confirm.";
    }
}

public class Program
{
    public static void Main()
    {
        var homeCareSystem = new HomeCareSystem();

        // SSM Staff
        var john = new Staff(1, "John Doe", "Nurse", "http://example.com/john.jpg");
        var jane = new Staff(2, "Jane Smith", "Cleaner", "http://example.com/jane.jpg");

        homeCareSystem.AddStaff(john);
        homeCareSystem.AddStaff(jane);

        // SS User
        var user = new User(1, "Alice", "123 Main St", "alice@example.com");
        user.Schedule.Add(new Schedule(user.Id, 1, new DateTime(2025, 3, 29, 10, 0, 0), "Nursing Care", "2 hours"));
        user.Schedule.Add(new Schedule(user.Id, 2, new DateTime(2025, 3, 29, 14, 0, 0), "Cleaning Service", "1 hour"));

        homeCareSystem.AddUser(user);

        // Example
        var scheduleForDay = homeCareSystem.ViewUserScheduleForDay(user, new DateTime(2025, 3, 29));
        foreach (var sched in scheduleForDay)
        {
            var staff = homeCareSystem.GetStaffById(sched.StaffId);
            Console.WriteLine($"Staff: {staff.Name} ({staff.Role})");
            Console.WriteLine($"Time: {sched.VisitTime}");
            Console.WriteLine($"Task: {sched.Task}");
            Console.WriteLine($"Duration: {sched.Duration}\n");
        }

        Console.WriteLine(homeCareSystem.SendReminder(user, new DateTime(2025, 3, 29, 10, 0, 0)));
        Console.WriteLine(homeCareSystem.RescheduleVisit(user, 1, 2));
        Console.WriteLine(homeCareSystem.ConfirmVisit(user, new DateTime(2025, 3, 29, 10, 0, 0)));
    }
}
