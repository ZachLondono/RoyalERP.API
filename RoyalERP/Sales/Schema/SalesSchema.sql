CREATE SCHEMA sales;

CREATE TABLE sales.companies (
    id uuid NOT NULL,
    version integer NOT NULL DEFAULT 0,
    name character varying(255) NOT NULL,
    CONSTRAINT companies_pkey PRIMARY KEY (id)
);

CREATE TABLE sales.addresses (
    id uuid NOT NULL,
    companyid uuid NOT NULL REFERENCES sales.companies(id) ON DELETE CASCADE,
    line1 character varying(255),
    line2 character varying(255),
    line3 character varying(255),
    city character varying(255),
    state character varying(255),
    zip character varying(255),
    CONSTRAINT addresses_pkey PRIMARY KEY (id)
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
    customerid uuid NOT NULL,
    vendorid uuid NOT NULL,
    CONSTRAINT orders_pkey PRIMARY KEY (id)
);