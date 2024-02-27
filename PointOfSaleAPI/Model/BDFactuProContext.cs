using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PointOfSaleAPI.Model
{
    public partial class BDFactuProContext : DbContext
    {
        public BDFactuProContext()
        {
        }

        public BDFactuProContext(DbContextOptions<BDFactuProContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Articulo> Articulos { get; set; } = null!;
        public virtual DbSet<ArticuloFactura> ArticuloFacturas { get; set; } = null!;
        public virtual DbSet<Factura> Facturas { get; set; } = null!;
        public virtual DbSet<Usuario> Usuarios { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Articulo>(entity =>
            {
                entity.ToTable("Articulo");

                entity.Property(e => e.Codigo)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.FechaActualizacion)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Precio).HasColumnType("decimal(14, 2)");
            });

            modelBuilder.Entity<ArticuloFactura>(entity =>
            {
                entity.HasKey(e => new { e.IdArticulo, e.IdFactura })
                    .HasName("PK__Articulo__FDF126FD7645E002");

                entity.ToTable("Articulo_Factura");

                entity.Property(e => e.FechaAgregado)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.MontoTotal).HasColumnType("decimal(14, 2)");

                entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(14, 2)");

                entity.HasOne(d => d.IdArticuloNavigation)
                    .WithMany(p => p.ArticuloFacturas)
                    .HasForeignKey(d => d.IdArticulo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Articulo___IdArt__46E78A0C");

                entity.HasOne(d => d.IdFacturaNavigation)
                    .WithMany(p => p.ArticuloFacturas)
                    .HasForeignKey(d => d.IdFactura)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Articulo___IdFac__47DBAE45");
            });

            modelBuilder.Entity<Factura>(entity =>
            {
                entity.ToTable("Factura");

                entity.Property(e => e.Consecutivo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FechaCreacion).HasColumnType("datetime");

                entity.Property(e => e.MontoTotal).HasColumnType("decimal(14, 2)");

                entity.Property(e => e.NumeroFactura)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Subtotal).HasColumnType("decimal(14, 2)");

                entity.HasOne(d => d.Vendedor)
                    .WithMany(p => p.Facturas)
                    .HasForeignKey(d => d.VendedorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Factura__Vendedo__4316F928");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuario");

                entity.HasIndex(e => e.Username, "UQ__Usuario__536C85E49967A0CB")
                    .IsUnique();

                entity.Property(e => e.ContrasenaHash)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ContrasenaSalt)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Token)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
