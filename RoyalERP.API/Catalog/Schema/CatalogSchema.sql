CREATE SCHEMA catalog;

CREATE TABLE catalog.productclasses (
    id uuid NOT NULL,
    name character varying(255) NOT NULL,
    CONSTRAINT productclasses_pkey PRIMARY KEY (id)
);

CREATE TABLE catalog.products (
    id uuid NOT NULL,
    classid uuid NOT NULL,
    name character varying(255) NOT NULL,
    CONSTRAINT products_pkey PRIMARY KEY (id)
);

CREATE TABLE catalog.productattributes (
    id uuid NOT NULL,
    productid uuid NOT NULL,
    name character varying(255) NOT NULL,
    CONSTRAINT productattributes_pkey PRIMARY KEY (id)
);

CREATE TABLE catalog.product_productattribute (
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 ),
    productid uuid NOT NULL REFERENCES catalog.products(id) ON DELETE CASCADE,
    attributeid uuid NOT NULL REFERENCES catalog.productattributes(id) ON DELETE CASCADE,,
    CONSTRAINT product_productattribute_pkey PRIMARY KEY (id)
);