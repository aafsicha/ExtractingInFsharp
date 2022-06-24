﻿namespace LiveCoding.Domain

open System
open System.Collections.Generic

type Capacity = {
    Value : int
    }

type Bar = {
    Name : string
    Capacity : Capacity
    OpenedDays : DayOfWeek[]
}

type Booking = {
    Bar : Bar
    Date : DateTime
}

type Dev = {
    Name : string
    OnSite : DateTime[]
}

type IBarRepository =
    abstract member Get : unit -> IEnumerable<Bar>
type IDevRepository =
    abstract member Get : unit -> IEnumerable<Dev>
type IBookingRepository =
    abstract member GetUpcomingBookings : unit -> IEnumerable<Booking>
    abstract member Save : Booking -> unit
    
module BarFunctions =
    let book (bar:Bar) (date:DateTime) =
        printfn "Bar booked: %s at %s" bar.Name (date.ToString())

    