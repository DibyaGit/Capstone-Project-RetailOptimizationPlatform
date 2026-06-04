using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailOptimizationPlatform.Api.Controllers
{
    /// <summary>
    /// Conceptual Model Context Protocol (MCP) API Controller.
    /// Provides endpoints for AI Agents (like Copilot/Gemini) to inspect retail issues
    /// and generate summarized action items. Fulfills Capstone US12 requirement.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class McpApiController : ControllerBase
    {
        private static readonly List<SupportTicket> MockTickets = new()
        {
            new SupportTicket { Id = 101, IssueType = "Inventory", Description = "Wireless Mouse stock level dropped below reorder point. Current stock: 5 units.", Priority = "High", DateCreated = DateTime.UtcNow.AddDays(-2) },
            new SupportTicket { Id = 102, IssueType = "Order", Description = "Customer Jane Smith reported that order amount ₹89.50 was debited twice on billing.", Priority = "Medium", DateCreated = DateTime.UtcNow.AddDays(-1) },
            new SupportTicket { Id = 103, IssueType = "Supplier", Description = "Keychron Inc delivery delayed by 4 days due to logistics bottleneck. Affects Mechanical Keyboard stock.", Priority = "High", DateCreated = DateTime.UtcNow.AddMinutes(-30) },
            new SupportTicket { Id = 104, IssueType = "General", Description = "Admin dashboard loading time is slow (avg 3.2 seconds) during peak hours.", Priority = "Low", DateCreated = DateTime.UtcNow.AddDays(-3) }
        };

        /// <summary>
        /// Retrieves the list of support tickets for the MCP AI Agent.
        /// </summary>
        [HttpGet("tickets")]
        public IActionResult GetTickets()
        {
            return Ok(MockTickets);
        }

        /// <summary>
        /// Conceptual AI summarization endpoint that mimics natural language processing.
        /// Generates a bulleted summarization and categorizes urgent tickets.
        /// </summary>
        [HttpPost("summarize")]
        public IActionResult SummarizeTickets([FromBody] List<int>? ticketIds)
        {
            var targets = ticketIds == null || !ticketIds.Any()
                ? MockTickets
                : MockTickets.Where(t => ticketIds.Contains(t.Id)).ToList();

            if (!targets.Any())
            {
                return BadRequest(new { Message = "No matching tickets found for summarization." });
            }

            var summaryBuilder = new StringBuilder();
            summaryBuilder.AppendLine($"### 🤖 AI Agent (MCP) Summarization Report ({DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC)");
            summaryBuilder.AppendLine($"Processed {targets.Count} tickets. Summary of findings:");
            summaryBuilder.AppendLine();

            foreach (var ticket in targets)
            {
                summaryBuilder.AppendLine($"- **[{ticket.Priority} Priority] Ticket #{ticket.Id} ({ticket.IssueType})**: {ticket.Description}");
            }

            summaryBuilder.AppendLine();
            summaryBuilder.AppendLine("#### Recommended Actions:");
            
            if (targets.Any(t => t.IssueType == "Inventory" && t.Priority == "High"))
            {
                summaryBuilder.AppendLine("1. ⚠️ **Urgent Stock Alert**: Reorder 'Wireless Mouse' immediately to prevent out-of-stock.");
            }
            if (targets.Any(t => t.IssueType == "Supplier" && t.Priority == "High"))
            {
                summaryBuilder.AppendLine("2. 🚚 **Supply Chain Delay**: Contact logistics team regarding 'Keychron Inc' delay.");
            }
            if (targets.Any(t => t.IssueType == "Order"))
            {
                summaryBuilder.AppendLine("3. 💳 **Finance Check**: Reconcile order double-charge report for customer Jane Smith.");
            }

            return Ok(new
            {
                SummaryText = summaryBuilder.ToString(),
                ProcessedCount = targets.Count,
                UrgentAlertsCount = targets.Count(t => t.Priority == "High")
            });
        }
    }

    public class SupportTicket
    {
        public int Id { get; set; }
        public string IssueType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = "Low";
        public DateTime DateCreated { get; set; }
    }
}
