CREATE SCHEMA manufacturing;

CREATE TABLE manufacturing.workorders (
    id uuid NOT NULL,
    version integer NOT NULL DEFAULT 0,
    number character varying(255) NOT NULL,
    name character varying(255) NOT NULL,
    customername character varying(255) NOT NULL,
    vendorname character varying(255) NOT NULL,
    releaseddate timestamp with time zone,
    scheduledddate timestamp with time zone,
    fulfilleddate timestamp with time zone,
    status character varying(255) NOT NULL,
    CONSTRAINT workorders_pkey PRIMARY KEY (id)
);