defmodule Nemestats.PageController do
  use Nemestats.Web, :controller

  def index(conn, _params) do
    render conn, "index.html"
  end
end
