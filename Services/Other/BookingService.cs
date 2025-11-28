using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using oculus_sport.Models;
using System.Diagnostics;

namespace oculus_sport.Services;

public class BookingService : IBookingService
{
    // In-memory list to store bookings while the app is running
    private readonly List<Booking> _bookings = new();

    // Hardcoded prices simulating database lookup
    private readonly Dictionary<string, decimal> _facilityBasePrices = new()
    {
        {"BadmintonCourt", 25.00m}, // RM 25.00
        {"PingPongCourt", 15.00m},  // RM 15.00
        {"BasketballCourt", 30.00m} // RM 30.00
    };

    // -----------------------------------------------------------
    // Core Business Logic (Implements IBookingService contract)
    // -----------------------------------------------------------

    /// <summary>
    /// Implements IBookingService.CalculateFinalCostAsync.
    /// Calculates the final cost based on facility, time slot, and user details.
    /// </summary>
    public async Task<string> CalculateFinalCostAsync(string facilityId, string timeSlot, string studentId)
    {
        await Task.Delay(100); // Simulate API call to fetch price

        decimal basePrice;

        // 1. Price Lookup Simulation
        if (!_facilityBasePrices.TryGetValue(facilityId.Replace(" ", "").Replace("1", "").Replace("2", "").Replace("3", ""), out basePrice))
        {
            basePrice = 20.00m; // Default price if not found (RM 20.00)
        }

        decimal finalCost = basePrice;

        // 2. Business Rule Simulation: Student Discount
        if (!string.IsNullOrEmpty(studentId) && studentId.Length > 5)
        {
            finalCost *= 0.9m; // 10% discount
            Debug.WriteLine($"Applied 10% student discount for ID: {studentId}");
        }

        // 3. Formatting
        // Format as Malaysian Ringgit (RM) with two decimal places
        return $"RM {finalCost:N2}";
    }

    /// <summary>
    /// Implements IBookingService.ProcessAndConfirmBookingAsync.
    /// Executes a simulated secure booking process. Checks memory for availability.
    /// </summary>
    public async Task<Booking?> ProcessAndConfirmBookingAsync(Booking newBooking)
    {
        await Task.Delay(500);

        if (_bookings.Any(b =>
            b.FacilityName == newBooking.FacilityName &&
            b.Date.Date == newBooking.Date.Date &&
            b.TimeSlot == newBooking.TimeSlot))
        {
            Debug.WriteLine("[Simulated Booking Failure] Slot already taken.");
            return null;
        }

        // Cost Calculation Integration
        newBooking.TotalCost = await CalculateFinalCostAsync(newBooking.FacilityName, newBooking.TimeSlot, newBooking.ContactStudentId);

        newBooking.Status = "Confirmed - In-Memory";
        newBooking.Date = newBooking.Date.Date;
        _bookings.Add(newBooking);

        Debug.WriteLine($"[Simulated Booking Success] Added booking {newBooking.Id} to memory. Cost: {newBooking.TotalCost}");
        return newBooking;
    }

    /// <summary>
    /// Implements IBookingService.GetUserBookingsAsync.
    /// Fetches all bookings for the current user from memory.
    /// </summary>
    public async Task<List<Booking>> GetUserBookingsAsync(string userId)
    {
        await Task.Delay(200);
        return _bookings.Where(b => b.UserId == userId).OrderBy(b => b.Date).ToList();
    }

    /// <summary>
    /// Implements IBookingService.GetAvailableTimeSlotsAsync.
    /// Fetches all time slots for a specific facility on a given date and marks them as available/booked.
    /// </summary>
    public async Task<IEnumerable<TimeSlot>> GetAvailableTimeSlotsAsync(string facilityId, DateTime date)
    {
        await Task.Delay(100);
        var masterSlots = GetMasterTimeSlots();
        var bookedSlotStrings = _bookings
            .Where(b => b.FacilityName == facilityId && b.Date.Date == date.Date)
            .Select(b => b.TimeSlot)
            .ToHashSet();

        return masterSlots
            .Select(ts => new TimeSlot
            {
                SlotName = ts.SlotName,
                IsAvailable = !bookedSlotStrings.Contains(ts.SlotName)
            })
            .ToList();
    }

    /// <summary>
    /// Implements IBookingService.IsSlotAvailableAsync.
    /// Quick check for availability for UI display.
    /// </summary>
    public async Task<bool> IsSlotAvailableAsync(string facilityId, string timeSlot, DateTime date)
    {
        await Task.Delay(50);
        return !_bookings.Any(b =>
            b.FacilityName == facilityId &&
            b.Date.Date == date.Date &&
            b.TimeSlot == timeSlot);
    }

    /// <summary>
    /// Implements IBookingService.UpdateBookingStatusAsync.
    /// Updates a booking status (e.g., Cancellation).
    /// </summary>
    public Task<bool> UpdateBookingStatusAsync(Booking booking, string newStatus)
    {
        var existing = _bookings.FirstOrDefault(b => b.Id == booking.Id);
        if (existing != null)
        {
            existing.Status = newStatus;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    private IEnumerable<TimeSlot> GetMasterTimeSlots()
    {
        var slots = new List<TimeSlot>();
        for (int hour = 8; hour < 22; hour++)
        {
            var start = new TimeSpan(hour, 0, 0);
            var end = new TimeSpan(hour + 1, 0, 0);
            slots.Add(new TimeSlot
            {
                SlotName = $"{start:hh\\:mm} - {end:hh\\:mm}",
                IsAvailable = true
            });
        }
        return slots;
    }
}