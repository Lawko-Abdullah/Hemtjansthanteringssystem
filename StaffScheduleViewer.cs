using System;
using System.Collections.Generic;
using System.Linq;

public class Task
{
    public int Id { get; set; }
    public string TaskDescription { get; set; }
    public DateTime ScheduledTime { get; set; }
    public string PatientName { get; set; }

    public Task(int id, string taskDescription, DateTime scheduledTime, string patientName)
    {
        Id = id;
        TaskDescription = taskDescription;
        ScheduledTime = scheduledTime;
        PatientName = patientName;
    }
}

public class Staff
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Task> Tasks { get; set; }

    public Staff(int id, string name)
    {
        Id = id;
        Name = name;
        Tasks = new List<Task>();
    }
}

public class HomeCareSystem
{
    private List<Staff> staffList;

    public HomeCareSystem()
    {
        staffList = new List<Staff>();
    }

    public void AddStaff(Staff staff)
    {
        staffList.Add(staff);
    }

    public Staff GetStaffById(int staffId)
    {
        return staffList.FirstOrDefault(staff => staff.Id == staffId);
    }

    public List<Task> ViewStaffSchedule(int staffId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var staff = GetStaffById(staffId);
        if (staff == null)
        {
            Console.WriteLine("Staff not found.");
            return new List<Task>();
        }

        var schedule = staff.Tasks;
        if (startDate.HasValue && endDate.HasValue)
        {
            schedule = schedule.Where(task => task.ScheduledTime >= startDate && task.ScheduledTime <= endDate).ToList();
        }

        return schedule;
    }

    public string ViewSchedule(int staffId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var schedule = ViewStaffSchedule(staffId, startDate, endDate);

        if (schedule.Count == 0)
        {
            return "No tasks assigned for today.";
        }

        foreach (var task in schedule)
        {
            Console.WriteLine($"Task: {task.TaskDescription}");
            Console.WriteLine($"Scheduled Time: {task.ScheduledTime}");
            Console.WriteLine($"Patient: {task.PatientName}\n");
        }
        return "Schedule viewed successfully.";
    }

    public string RefreshSchedule(int staffId)
    {
        var staff = GetStaffById(staffId);
        if (staff != null)
        {
            // Simulate a schedule update 
            staff.Tasks.Add(new Task(3, "Administer Medication", DateTime.Now.AddHours(3), "Patient C"));
            return "Schedule updated. New tasks assigned.";
        }
        return "Unable to update schedule. Staff not found.";
    }

    public string SendReminder(int staffId)
    {
        var schedule = ViewStaffSchedule(staffId);
        foreach (var task in schedule)
        {
            var timeLeft = task.ScheduledTime - DateTime.Now;
            if (timeLeft.TotalMinutes <= 60)
            {
                return $"Reminder: Task '{task.TaskDescription}' for {task.PatientName} at {task.ScheduledTime}.";
            }
        }
        return "No tasks requiring reminders.";
    }
}

public class Program
{
    public static void Main()
    {
        var homeCareSystem = new HomeCareSystem();

        // Adding staff  tasks
        var staff1 = new Staff(1, "John Doe");
        staff1.Tasks.Add(new Task(1, "Nursing Care", DateTime.Now.AddHours(1), "Patient A"));
        staff1.Tasks.Add(new Task(2, "Cleaning", DateTime.Now.AddHours(2), "Patient B"));
        homeCareSystem.AddStaff(staff1);

        // View Schedule (default )
        Console.WriteLine("Viewing Schedule for John Doe (Staff 1):");
        Console.WriteLine(homeCareSystem.ViewSchedule(1));

        //  reminder for upcoming task
        Console.WriteLine("\nSending reminder for upcoming task:");
        Console.WriteLine(homeCareSystem.SendReminder(1));

        // Simulate a schedule update
        Console.WriteLine("\nRefreshing Schedule (simulating task update):");
        Console.WriteLine(homeCareSystem.RefreshSchedule(1));
        Console.WriteLine(homeCareSystem.ViewSchedule(1));

        // Viewing schedule
        Console.WriteLine("\nViewing schedule for specific date range (today):");
        var startDate = DateTime.Now.Date;
        var endDate = DateTime.Now.Date.AddDays(1); // Next day
        Console.WriteLine(homeCareSystem.ViewSchedule(1, startDate, endDate));
    }
}
