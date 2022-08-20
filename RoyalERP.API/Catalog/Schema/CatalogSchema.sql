CREATE SCHEMA catalog;

CREATE TABLE catalog.productclasses (
    id uuid NOT NULL,
    version integer NOT NULL DEFAULT 0,
    name character varying(255) NOT NULL,
    CONSTRAINT productclasses_pkey PRIMARY KEY (id)
);

CREATE TABLE catalog.productattributes (
    id uuid NOT NULL,
    version integer NOT NULL DEFAULT 0,
    name character varying(255) NOT NULL,
    CONSTRAINT productattributes_pkey PRIMARY KEY (id)
);

CREATE TABLE catalog.products (
    id uuid NOT NULL,
    version integer NOT NULL DEFAULT 0,
    classid uuid REFERENCES catalog.productclasses(id),
    name character varying(255) NOT NULL,
    attributeids uuid[] NOT NULL,
    CONSTRAINT products_pkey PRIMARY KEY (id)
);