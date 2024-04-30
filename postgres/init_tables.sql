create schema if not exists kanafka;

create table if not exists kanafka.failed_messages(
    id uuid primary key not null,
	failed_on timestamp not null,
	state int4 default 1 not null,
	topic text not null,
	message_id uuid not null,
	message_body jsonb,
	message_headers jsonb,
	exception_type text,
	exception_message text,
	exception_stack_trace text,
    inner_exception_type text,
	inner_exception_message text,
	retries int4 default 0 not null,
	archived_on timestamp
);

create table if not exists kanafka.delayed_messages (
	id uuid primary key not null,
	produce_on timestamp not null,
	topic text,
	message_body jsonb,
	message_headers jsonb,
	is_produced bool default false not null
);
create index if not exists ix_delayed_messages_produce_on_is_produced on kanafka.delayed_messages (produce_on, is_produced);