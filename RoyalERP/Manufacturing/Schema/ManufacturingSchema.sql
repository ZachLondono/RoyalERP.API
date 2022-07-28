CREATE SCHEMA manufacturing;

CREATE TABLE manufacturing.workorders (
    id uuid NOT NULL,
    number character varying(255) NOT NULL,
    name character varying(255) NOT NULL,
    releaseddate timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    fulfilleddate timestamp with time zone,
    status character varying(255) NOT NULL,
    CONSTRAINT companies_pkey PRIMARY KEY (id)
);