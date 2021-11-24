USE Supermercado;
INSERT INTO Rubro   (idRubro, rubro)
            VALUES  (1, 'Gaseosa'),
                    (2, 'Lacteo');

INSERT INTO Producto    (idProducto, idRubro, nombre, cantidad, precioUnitario)
            VALUES      (10,    1, 'Manaos Cola 2.25L.', 500, 69.5),
                        (20,    1, 'Cunnington Cola 2.25L.', 1000, 90);