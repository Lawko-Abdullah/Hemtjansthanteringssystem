using System;
using System.Collections.Generic;
using System.Linq;

public class Service
{
    public string Description { get; set; }
    public DateTime ServiceDate { get; set; }
    public decimal Cost { get; set; }

    public Service(string description, DateTime serviceDate, decimal cost)
    {
        Description = description;
        ServiceDate = serviceDate;
        Cost = cost;
    }
}

public class Client
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Service> ServicesPerformed { get; set; }

    public Client(int id, string name)
    {
        Id = id;
        Name = name;
        ServicesPerformed = new List<Service>();
    }
}

public class Invoice
{
    public int InvoiceNumber { get; set; }
    public Client Client { get; set; }
    public List<Service> Services { get; set; }
    public decimal TotalCost { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal TotalAmountDue { get; set; }

    public Invoice(int invoiceNumber, Client client)
    {
        InvoiceNumber = invoiceNumber;
        Client = client;
        Services = client.ServicesPerformed;
        Discount = 0;
        Tax = 0.1m; // 10% tax
    }

    public void CalculateTotal()
    {
        TotalCost = Services.Sum(s => s.Cost);
        TotalAmountDue = TotalCost - Discount + (TotalCost * Tax);
    }

    public void ApplyDiscount(decimal discountAmount)
    {
        Discount = discountAmount;
        CalculateTotal();
    }

    public string GetInvoiceDetails()
    {
        string serviceDetails = string.Join("\n", Services.Select(s => $"Service: {s.Description}, Date: {s.ServiceDate}, Cost: {s.Cost:C}"));
        return $@"
Invoice Number: {InvoiceNumber}
Client: {Client.Name}
Services Performed:
{serviceDetails}

Total Cost: {TotalCost:C}
Discount: {Discount:C}
Tax: {Tax * 100}%
Total Amount Due: {TotalAmountDue:C}
";
    }
}

public class BillingSystem
{
    private List<Client> clients;
    private int nextInvoiceNumber;

    public BillingSystem()
    {
        clients = new List<Client>();
        nextInvoiceNumber = 1001; // Starting Invoice number
    }

    public void AddClient(Client client)
    {
        clients.Add(client);
    }

    public Client GetClientById(int clientId)
    {
        return clients.FirstOrDefault(c => c.Id == clientId);
    }

    public string GenerateInvoice(int clientId, decimal discountAmount = 0)
    {
        var client = GetClientById(clientId);

        if (client == null)
        {
            return "Client not found.";
        }

        if (client.ServicesPerformed.Count == 0)
        {
            return "No services were performed during this period for this client.";
        }

        var invoice = new Invoice(nextInvoiceNumber++, client);
        if (discountAmount > 0)
        {
            invoice.ApplyDiscount(discountAmount);
        }

        return invoice.GetInvoiceDetails();
    }
}

public class Program
{
    public static void Main()
    {
        // Create Billing System
        var billingSystem = new BillingSystem();

        // Add  clients and their services
        var client1 = new Client(1, "John Doe");
        client1.ServicesPerformed.Add(new Service("Nursing Care", DateTime.Now.AddDays(-2), 50));
        client1.ServicesPerformed.Add(new Service("House Cleaning", DateTime.Now.AddDays(-1), 30));
        billingSystem.AddClient(client1);

        var client2 = new Client(2, "Jane Smith");
        client2.ServicesPerformed.Add(new Service("Physical Therapy", DateTime.Now.AddDays(-1), 80));
        billingSystem.AddClient(client2);

        // Generate invoice f
        Console.WriteLine("Generating Invoice for John Doe:");
        Console.WriteLine(billingSystem.GenerateInvoice(1, 10));  // Applying a 10 unit discount

        // Generate invoice for client 2 without any discount
        Console.WriteLine("\nGenerating Invoice for Jane Smith:");
        Console.WriteLine(billingSystem.GenerateInvoice(2));
        
        // Case where no services are performed
        var client3 = new Client(3, "Mary Johnson");
        billingSystem.AddClient(client3);
        Console.WriteLine("\nGenerating Invoice for Mary Johnson (No services):");
        Console.WriteLine(billingSystem.GenerateInvoice(3));
    }
}
