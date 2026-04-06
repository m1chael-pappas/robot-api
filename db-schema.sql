--
-- PostgreSQL database dump
--

\restrict LZ6XIJ63saq3BMQcwT3sjINt2BfDYysNkXnvSbcznAOmZhtHFaxhunHCRzxgGTO

-- Dumped from database version 14.22 (Debian 14.22-1.pgdg13+1)
-- Dumped by pg_dump version 14.22 (Debian 14.22-1.pgdg13+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: map; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.map (
    id integer NOT NULL,
    columns integer NOT NULL,
    rows integer NOT NULL,
    "Name" character varying(50) NOT NULL,
    description character varying(800),
    createddate timestamp without time zone NOT NULL,
    modifieddate timestamp without time zone NOT NULL,
    issquare boolean GENERATED ALWAYS AS (((rows > 0) AND (rows = columns))) STORED
);


ALTER TABLE public.map OWNER TO postgres;

--
-- Name: map_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.map ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.map_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: robotcommand; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.robotcommand (
    id integer NOT NULL,
    "Name" character varying(50) NOT NULL,
    description character varying(800),
    ismovecommand boolean NOT NULL,
    createddate timestamp without time zone NOT NULL,
    modifieddate timestamp without time zone NOT NULL
);


ALTER TABLE public.robotcommand OWNER TO postgres;

--
-- Name: robotcommand_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.robotcommand ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.robotcommand_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: user; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."user" (
    id integer NOT NULL,
    email character varying(100) NOT NULL,
    firstname character varying(50) NOT NULL,
    lastname character varying(50) NOT NULL,
    passwordhash character varying(500) NOT NULL,
    description character varying(800),
    role character varying(50),
    createddate timestamp without time zone NOT NULL,
    modifieddate timestamp without time zone NOT NULL
);


ALTER TABLE public."user" OWNER TO postgres;

--
-- Name: user_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."user" ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.user_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Data for Name: map; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.map (id, columns, rows, "Name", description, createddate, modifieddate) FROM stdin;
1	5	5	DEAKIN	Default 5x5 square map	2022-07-30 00:00:00	2022-07-30 00:00:00
2	10	10	MOON	Large 10x10 square map	2022-07-30 00:00:00	2022-07-30 00:00:00
3	5	10	BURWOOD	Rectangular 5x10 map	2022-07-30 00:00:00	2022-07-30 00:00:00
\.


--
-- Data for Name: robotcommand; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.robotcommand (id, "Name", description, ismovecommand, createddate, modifieddate) FROM stdin;
1	LEFT	Turn the robot 90 degrees left	t	2022-07-30 00:00:00	2022-07-30 00:00:00
2	RIGHT	Turn the robot 90 degrees right	t	2022-07-30 00:00:00	2022-07-30 00:00:00
3	MOVE	Move the robot one step forward	t	2022-07-30 00:00:00	2022-07-30 00:00:00
4	PLACE	Place the robot at X,Y facing direction D	f	2022-07-30 00:00:00	2022-07-30 00:00:00
5	REPORT	Report the current position of the robot	f	2022-07-30 00:00:00	2022-07-30 00:00:00
\.


--
-- Data for Name: user; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."user" (id, email, firstname, lastname, passwordhash, description, role, createddate, modifieddate) FROM stdin;
1	admin@deakin.edu.au	Admin	User	AQAAAAIAAYagAAAAEAIcsGUs1W8GPnpnpuR7eBzAFVrE1OiExwfkB4VVlcDAf8oh4Gc7q+ThFCBFrATSpw==	Administrator account	Admin	2022-07-30 00:00:00	2022-07-30 00:00:00
2	user@deakin.edu.au	Regular	User	AQAAAAIAAYagAAAAEMxnyQZ74TxLEtBJ6WTuHWQ0xcLucsshoQRuwwenEuqjCgBjmWHADOQnTzDwZs1qRw==	Regular user account	User	2022-07-30 00:00:00	2022-07-30 00:00:00
\.


--
-- Name: map_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.map_id_seq', 3, true);


--
-- Name: robotcommand_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.robotcommand_id_seq', 5, true);


--
-- Name: user_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.user_id_seq', 2, true);


--
-- PostgreSQL database dump complete
--

\unrestrict LZ6XIJ63saq3BMQcwT3sjINt2BfDYysNkXnvSbcznAOmZhtHFaxhunHCRzxgGTO

