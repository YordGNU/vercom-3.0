using System;

namespace vercom.Interfaces
{
    public class iPVAnalitic
    {
        public int id { get; set; }
        public int lnk_entrada { get; set; }
        public int lnk_venta { get; set; }
        public int lnk_devol { get; set; }
        public int lnk_merma { get; set; }
        public string cod { get; set; }
        public string producto { get; set; }
        public DateTime fecha { get; set; }
        public string fechaV { get; set; }
        public string unidad { get; set; }
        public double? cantidad_saldo { get; set; }
        public double? importe_saldo { get; set; }
        public double? cantidad_entrada { get; set; }
        public double? importe_entrada { get; set; }
        public double? cantidad_venta { get; set; }
        public double? precio_venta { get; set; }
        public double? importe_venta { get; set; }
        public double? cantidad_devolucion { get; set; }
        public double? importe_devolucion { get; set; }
        public double? cantidad_merma { get; set; }
        public double? importe_merma { get; set; }
        public double? final_saldo { get; set; }
        public double? final_importe { get; set; }
        public string via_pago { get; set; }
        public int via_pagocant { get; set; }
        public string punto_nombre { get; set; }
        public Boolean? concilia { get; set; }
        public double? diferencia { get; set; }
    }
}