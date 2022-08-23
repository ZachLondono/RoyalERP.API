CREATE SCHEMA manufacturing;

CREATE TABLE manufacturing.workorders (
    id uuid NOT NULL,
    version integer NOT NULL DEFAULT 0,
    salesorderid uuid NOT NULL,
    number character varying(255) NOT NULL,
    name character varying(255) NOT NULL,
    note character varying(255) NOT NULL,
    productclass uuid NOT NULL,
    quantity integer NOT NULL,
    customername character varying(255) NOT NULL,
    vendorname character varying(255) NOT NULL,
    releaseddate timestamp with time zone,
    scheduledddate timestamp with time zone,
    fulfilleddate timestamp with time zone,
    scheduleddate timestamp with time zone,
    status character varying(255) NOT NULL,
    CONSTRAINT workorders_pkey PRIMARY KEY (id)
);