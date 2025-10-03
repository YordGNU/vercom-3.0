using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vercom.Interfaces
{
    public class iCodificadores
    {
        public List<CategoriasDTO> categorias;
        public List<SubCategoriasDTO> subCategorias;
        public List<UnidadesDTO> unidades;
        public List<TOperacionesDTO> t_operaciones;     
        public List<TPagosDTO> t_pagos;
        public List<TFacturaDTO> t_facturas;
        public List<TClienteDTO> t_clientes;
        public List<FOperacionDTO> f_operacion;

        public iCodificadores(List<CategoriasDTO> categorias, List<SubCategoriasDTO> subCategorias, List<UnidadesDTO> unidades, List<TOperacionesDTO> operaciones, List<TPagosDTO> pagos, List<TFacturaDTO> facturas, List<TClienteDTO> clientes, List<FOperacionDTO> f_operacion)
        {
            this.categorias = categorias;
            this.subCategorias = subCategorias;
            this.unidades = unidades;
            t_operaciones = operaciones;
            t_pagos = pagos;
            t_facturas = facturas;
            t_clientes = clientes;
            this.f_operacion = f_operacion;
        }
    }

    public class CategoriasDTO
    {
        public int ID { get; set; }
        public string Clave { get; set; }
    }
    public class SubCategoriasDTO
    {
        public int ID { get; set; }
        public string Clave { get; set; }
    }
    public class UnidadesDTO
    {
        public int ID { get; set; } = 0;        
        public string Clave { get; set; }
    }
    public class TOperacionesDTO
    {
        public int ID { get; set; } 
        public string Clave { get; set; }
    }
    public class TPagosDTO
    {
        public int ID { get; set; }
        public string Clave { get; set; }
    }
    public class TFacturaDTO
    {
        public int ID { get; set; }
        public string Clave { get; set; }
    }
    public class TClienteDTO
    {
        public int ID { get; set; }
        public string Clave { get; set; }
    }
    public class FOperacionDTO
    {
        public int ID { get; set; }
        public string Clave { get; set; }
    }

}