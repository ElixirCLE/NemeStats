# This file is responsible for configuring your application
# and its dependencies with the aid of the Mix.Config module.
#
# This configuration file is loaded before any dependency and
# is restricted to this project.
use Mix.Config

# General application configuration
config :nemestats,
  ecto_repos: [Nemestats.Repo]

# Configures the endpoint
config :nemestats, Nemestats.Endpoint,
  url: [host: "localhost"],
  secret_key_base: "QEFIclwXg6tgZ71oPzNFPy3IbEJm8r/U3uKd0w9LErLU0ff8Etfg6WEO3jQ2Q3f/",
  render_errors: [view: Nemestats.ErrorView, accepts: ~w(html json)],
  pubsub: [name: Nemestats.PubSub,
           adapter: Phoenix.PubSub.PG2]

# Configures Elixir's Logger
config :logger, :console,
  format: "$time $metadata[$level] $message\n",
  metadata: [:request_id]

# Import environment specific config. This must remain at the bottom
# of this file so it overrides the configuration defined above.
import_config "#{Mix.env}.exs"
