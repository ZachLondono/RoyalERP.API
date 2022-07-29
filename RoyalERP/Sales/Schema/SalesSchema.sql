CREATE SCHEMA sales;

CREATE TABLE sales.companies (
    id uuid NOT NULL,
    version integer NOT NULL DEFAULT 0,
    name character varying(255) NOT NULL,
    CONSTRAINT companies_pkey PRIMARY KEY (id)
);

CREATE TABLE sales.orders (
    id uuid NOT NULL,
    version integer NOT NULL DEFAULT 0,
    number character varying(255) NOT NULL,
    name character varying(255) NOT NULL,
    placeddate timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    confirmeddate timestamp with time zone,
    completeddate timestamp with time zone,
    status character varying(255) NOT NULL,
    CONSTRAINT orders_pkey PRIMARY KEY (id)
);