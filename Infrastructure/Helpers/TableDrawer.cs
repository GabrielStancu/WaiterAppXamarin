﻿using Core.Helpers;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Infrastructure.Helpers
{
    public class TableDrawer
    {
        public IEnumerable<DrawnTable> DrawTables(IEnumerable<Table> tables, IEnumerable<Order> orders,
            int waiterId, int initialImageHeight, int initialImageWidth)
        {          
            var drawnTables = new List<DrawnTable>();
            (double screenWidth, double screenHeight) = new DeviceInfoCollector().GetScreenDimensions();
            double heightRatio = screenHeight / initialImageHeight;
            double widthRatio = screenWidth / initialImageWidth;
            int xOffset = (int)(screenWidth / 15);
            int yOffset = (int)(screenHeight / 15);

            foreach (var table  in tables)
            {
                var drawnTable = new DrawnTable()
                {
                    TableNumber = table.TableNumber,
                    Color = GetTableColor(table, orders, waiterId),
                    Total = GetOrderTotal(table, orders),
                    WaiterName = table.Waiter?.FirstName,
                    StartX = (int)(table.StartX * widthRatio) - xOffset,
                    StartY = (int)(table.StartY * heightRatio) - yOffset,
                    LengthX = table.LengthX,
                    LengthY = table.LengthY
                };

                drawnTables.Add(drawnTable);
            }

            return drawnTables;
        }

        private double GetOrderTotal(Table table, IEnumerable<Order> orders)
        {
            var crtTableOrder = orders.FirstOrDefault(o => o.TableId == table.Id);

            return crtTableOrder?.Total ?? 0;
        }

        private Color GetTableColor(Table table, IEnumerable<Order> orders, int waiterId)
        {
            var crtTableOrder = orders.FirstOrDefault(o => o.TableId == table.Id);

            if(crtTableOrder is null)
            {
                return Color.Green;
            }

            if(waiterId == crtTableOrder.WaiterId)
            {
                return Color.Blue;
            }

            return Color.Red;
        }
    }
}