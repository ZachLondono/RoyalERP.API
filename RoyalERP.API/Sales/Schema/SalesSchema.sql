CREATE SCHEMA sales;

CREATE TABLE sales.companies (
    id uuid NOT NULL,
    version integer NOT NULL DEFAULT 0,
    name character varying(255) NOT NULL,
    contact character varying(255) NOT NULL,
    email character varying(255) NOT NULL,
    info jsonb NOT NULL DEFAULT '{}'::jsonb,
    CONSTRAINT companies_pkey PRIMARY KEY (id)
);

CREATE TABLE sales.companydefaults (
    id uuid NOT NULL,
    companyid uuid NOT NULL,
    productid uuid NOT NULL,
    attributeid uuid NOT NULL,
    value varchar(255) NOT NULL, 
    CONSTRAINT companydefaults_pkey PRIMARY KEY (id)
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

CREATE TABLE sales.ordereditems (
    id uuid NOT NULL,
    orderid uuid NOT NULL REFERENCES sales.orders(id) ON DELETE CASCADE,
    productid uuid NOT NULL,
    productname character varying(255) NOT NULL,
    quantity integer NOT NULL DEFAULT 0,
    properties jsonb NOT NULL,
    CONSTRAINT ordereditem_pkey PRIMARY KEY (id)
);